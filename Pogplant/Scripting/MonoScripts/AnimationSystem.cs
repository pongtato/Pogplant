using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class AnimationSystem
    {
        public int queue_state_index;

        public struct Animation_Stack
        {
            public Action action;
            public float animation_duration;
            public bool loop_animation;
        }

        //Animation stack
        List<Animation_Stack> animation_specs_stack;
        List<Action<float>> animation_update_stack;
        Action<string> queue_state_func;
        string queue_state;
        float current_animation_time;
        int current_animation_index;
        bool play_animation;

        public void Init()
        {
            animation_specs_stack = new List<Animation_Stack>();
            animation_update_stack = new List<Action<float>>();
            current_animation_index = 0;
        }

        public void Update(float dt)
        {
            if (animation_update_stack.Count > 0 && play_animation)
            {
                animation_update_stack[current_animation_index].Invoke(dt);

                //Go to the next stack in the animation if the duration exceeds
                if (current_animation_time >= animation_specs_stack[current_animation_index].animation_duration)
                {
                    RunNextAnimationStack();
                }
                else
                {
                    //Only add the timer if the stack has something inside
                    if (animation_specs_stack.Count > 0)
                    {
                        current_animation_time += dt;
                    }
                }
            }
        }

        #region[State & animation queue]
        public void SetStateQueue(Action<string> set_state_func, string state)
        {
            queue_state_func = set_state_func;
            queue_state = state;
        }

        public void ResetStateQueue()
        {
            queue_state_func = null;
            queue_state = "";
        }

        public void AddAnimationSpecsStack(Action action, float anim_duration, bool loop = false)
        {
            animation_specs_stack.Add(new Animation_Stack()
            {
                action = action,
                animation_duration = anim_duration,
                loop_animation = loop
            });
        }

        public void AddAnimationUpdateStack(Action<float> action)
        {
            animation_update_stack.Add(action);
        }

        public void PlayAnimation(int index = 0)
        {
            if (index < animation_specs_stack.Count)
            {
                current_animation_index = index;
                current_animation_time = 0.0f;
                animation_specs_stack[current_animation_index].action.Invoke();
                play_animation = true;
            }
        }

        public void StopAnimation(bool clear_stack, Dictionary<uint, MovingParts> moving_parts_dict)
        {
            play_animation = false;

            if (clear_stack)
                ClearAnimationStack(moving_parts_dict);

            //if there is a queue of animation state to transition to, transition to that state
            if (queue_state_func != null)
            {
                queue_state_func.Invoke(queue_state);
                ResetStateQueue();
            }
        }

        public void ClearAnimationStack(Dictionary<uint, MovingParts> moving_parts_dict)
        {
            current_animation_index = 0;
            current_animation_time = 0.0f;

            //Reset all variables
            ResetAnimationMovingPart(moving_parts_dict);

            //Use the function so its easier to track
            animation_specs_stack.Clear();
            animation_update_stack.Clear();
        }

        public void ResetAnimationMovingPart(Dictionary<uint, MovingParts> moving_parts_dict)
        {
            foreach (KeyValuePair<uint, MovingParts> kvp in moving_parts_dict)
            {
                kvp.Value.ResetAllVariables();
            }
        }

        void RunNextAnimationStack()
        {
            play_animation = true;
            current_animation_time = 0.0f;

            //Increase the index if not looping
            if (animation_specs_stack[current_animation_index].loop_animation == false)
            {
                ++current_animation_index;
            }

            if (current_animation_index <= animation_specs_stack.Count - 1)
            {
                //Only run if the stack still has actions
                if (animation_specs_stack[current_animation_index].action != null)
                {
                    animation_specs_stack[current_animation_index].action.Invoke();
                }
            }
            else
            {
                //Stop playing once the end of stack and end of animation is reached
                play_animation = false;

                //if there is a queue of animation state to transition to, transition to that state
                if (queue_state_func != null)
                {
                    queue_state_func.Invoke(queue_state);
                    ResetStateQueue();
                }
            }
        }
        #endregion
    }
}
