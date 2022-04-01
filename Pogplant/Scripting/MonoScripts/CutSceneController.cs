using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scripting.Utilities;

namespace Scripting
{
    public class CutSceneController : MonoBehaviour
    {

        float overall_acc_dt = 0.0f;
        uint CS_Board = 0;
        Vector3 InitialCanvasPos = new Vector3(0, -0.3f, 0);
        Vector3 InitialCamPos = new Vector3(-0.28f, 0.38f, 5);
        float InitialFOV = 2.4f;
        float HeldDownAccu = 0f;

        bool bIsInputHeldDown = false;
        bool bCanLoadCutScene = false;

        public int subs_begin_index;        //For kb/mouse
        public int subs_end_index;
        public int subs_controller_begin_index;
        public string subs_level_id;
        uint sub_renderer_id;
        public CutSceneController() 
        {

        }

        public override void Start()
        {
            ECS.PlaySubtitles(sub_renderer_id);
        }

        public override void Init(ref uint _entityID)
        {
            //Cam is the self id
            entityID = _entityID;
            CS_Board = ECS.FindEntityWithName("CS_Canvas");
            

            ECS.SetGlobalPosition(CS_Board, InitialCanvasPos);
            ECS.SetGlobalPosition(entityID, InitialCamPos);
            ECS.SetFOV(entityID, InitialFOV);
            ECS.PlayAudio(entityID, 0, "VO");

            subs_begin_index = ECS.GetValue<int>(entityID, 0, "SubBeginIndex");
            subs_controller_begin_index = ECS.GetValue<int>(entityID, 0, "SubCtrlerBeginIndex");
            subs_end_index = ECS.GetValue<int>(entityID, 1, "SubEndIndex");
            subs_level_id = GameUtilities.GetSceneName();
            sub_renderer_id = ECS.FindEntityWithName("Subs_Renderer");

            ECS.SetSubtitles(sub_renderer_id, subs_level_id, subs_controller_begin_index, subs_end_index);
        }

        public override void Update(float dt)
        {
            if (InputUtility.onKeyHeld(KEY_ID.KEY_ESCAPE))
            {
                bIsInputHeldDown = true;
            }
            else
            {
                bIsInputHeldDown = false;
                HeldDownAccu = 0;
            }

            //For the first 0.5 sec
            overall_acc_dt += dt;
            Phase_CS_TP(0, 0.5f, new Vector3(0, 0.03f, 0));
            Phase_CS_TP(0.5f, 0.9f, new Vector3(0, -0.03f, 0));
            Phase_CS_TP(0.9f, 1.3f, new Vector3(0, 0, 0));

            Phase_Cam_FOV(1.3f, 6.0f, 2.4f, 2.2f);
            Phase_Cam_POS(6.0f, 16.0f, InitialCamPos,  new Vector3(0.114f, 0.38f, 5));
            Phase_Cam_POS(16.0f, 19.0f, new Vector3(0.114f, 0.38f, 5), new Vector3(0.114f, 0.385f, 5));
            //Meteor crash into backyard at 17s with woosh sound
            Phase_Cam_TP_FOV(18.5f, 20.0f, 2.2f, 1.9f, 5);
            Phase_Cam_TP_POS(18.5f, 20.0f, new Vector3(0.114f, 0.385f, 5), new Vector3(0.34f, 0.375f, 5), 5);

            Phase_Cam_FOV(20.0f, 25.5f, 1.9f, 1.7f);
            Phase_Cam_POS(20.0f, 25.5f, new Vector3(0.34f, 0.375f, 5), new Vector3(0.365f, 0.115f, 5));

            Phase_Cam_FOV(29.0f, 34.0f, 1.7f, 1.4f);
            Phase_Cam_POS(29.0f, 34.0f, new Vector3(0.365f, 0.115f, 5), new Vector3(0.155f, 0.158f, 5));
            //Enemy portrait shown at 38s with woosh sound
            Phase_Cam_TP_FOV(38.5f, 40.0f, 1.4f, 2.35f, 5);
            Phase_Cam_TP_POS(38.5f, 40.0f, new Vector3(0.155f, 0.158f, 5), new Vector3(-0.07f, 0.126f, 5), 5);


            Phase_Cam_FOV(44.0f, 45.0f, 2.35f, 1.6f);
            Phase_Cam_POS(44.0f, 45.0f, new Vector3(-0.07f, 0.126f, 5), new Vector3(-0.32f, 0.19f, 5));
            Phase_Cam_FOV(45.0f, 47.0f, 1.6f, 1.4f);

            Phase_Cam_POS(48.0f, 54.0f, new Vector3(-0.32f, 0.19f, 5), new Vector3(-0.225f, -0.08f, 5));
            Phase_Cam_FOV(48.0f, 54.0f, 1.4f, 1.63f);
            //Scientist pointing shown at 54s with woosh sound
            Phase_Cam_TP_POS(54.5f, 55.0f, new Vector3(-0.225f, -0.08f, 5), new Vector3(-0.375f, -0.170f, 5),5);
            Phase_Cam_TP_FOV(54.5f, 55.0f, 1.63f, 1.6f, 5);

            Phase_Cam_POS(55.0f, 57.0f, new Vector3(-0.375f, -0.170f, 5), new Vector3(-0.390f, -0.135f, 5));
            Phase_Cam_FOV(55.0f, 57.0f, 1.6f, 1.4f);
            //Scientist pointing shown at 57s with woosh sound
            Phase_Cam_TP_POS(57.5f, 58.0f, new Vector3(-0.390f, -0.135f, 5), new Vector3(-0.240f, -0.22f, 5), 5);

            Phase_Cam_POS(58.0f, 59.0f, new Vector3(-0.240f, -0.22f, 5), new Vector3(-0.230f, -0.22f, 5));
            Phase_Cam_FOV(58.0f, 59.0f, 1.4f, 1.2f);

            Phase_Cam_POS(59.0f, 60.0f, new Vector3(-0.230f, -0.22f, 5), new Vector3(-0.240f, -0.225f, 5));
            Phase_Cam_FOV(59.0f, 60.0f, 1.2f, 1.1f);
            //"Lets get to work" Whoosh sound at 60s
            Phase_Cam_TP_POS(60.5f, 61.0f, new Vector3(-0.240f, -0.225f, 5), new Vector3(-0.300f, -0.420f, 5));
            Phase_Cam_TP_FOV(60.5f, 61.0f, 1.1f, 1.6f);

            Phase_Cam_POS(61f, 62f, new Vector3(-0.300f, -0.420f, 5), new Vector3(-0.310f, -0.412f, 5));
            Phase_Cam_FOV(61f, 62f, 1.6f, 1.2f);
            //Music start
            Phase_Cam_TP_POS(62f, 63f, new Vector3(-0.310f, -0.412f, 5), new Vector3(-0.05f, -0.36f, 5));
            Phase_Cam_TP_FOV(62f, 63f, 1.2f, 1.6f);

            Phase_Cam_POS(63f, 64.5f, new Vector3(-0.05f, -0.36f, 5), new Vector3(-0.02f, -0.358f, 5));
            Phase_Cam_FOV(63f, 64.5f, 1.6f, 1.8f);

            Phase_Cam_POS(64.5f, 66f, new Vector3(-0.02f, -0.358f, 5), new Vector3(-0.01f, -0.332f, 5));
            Phase_Cam_FOV(64.5f, 66f, 1.8f, 2.1f);

            Phase_Cam_POS(66f, 66.7f, new Vector3(-0.01f, -0.332f, 5), new Vector3(0.017f, -0.324f, 5));
            Phase_Cam_FOV(66f, 66.7f, 2.1f, 2.2f);

            Phase_Cam_TP_POS(66.7f, 67.3f, new Vector3(0.017f, -0.324f, 5), new Vector3(0.017f, -0.105f, 5));
            Phase_Cam_TP_FOV(66.7f, 67.3f, 2.2f, 1.8f);

            Phase_Cam_POS(67.3f, 70f, new Vector3(0.017f, -0.105f, 5), new Vector3(0.017f, -0.08f, 5));
            //AAAANND DONE part
            Phase_Cam_TP_POS(70f, 70.8f, new Vector3(0.017f, -0.08f, 5), new Vector3(0.24f, -0.095f, 5));
            Phase_Cam_TP_FOV(70f, 70.8f, 1.8f, 1.4f);

            Phase_Cam_TP_POS(71.3f, 72f, new Vector3(0.24f, -0.095f, 5), new Vector3(0.3f, -0.2f, 5));
            Phase_Cam_TP_FOV(71.3f, 72f, 1.4f, 2.22f);

            Phase_Cam_POS(72.0f, 79f, new Vector3(0.3f, -0.2f, 5), new Vector3(0.32f, -0.39f, 5));

            Phase_Cam_POS(81.0f, 82f, new Vector3(0.32f, -0.39f, 5), new Vector3(0.32f, -0.6f, 5));

            if(bIsInputHeldDown)
            {
                HeldDownAccu += dt;
                if(HeldDownAccu > 2.0f)
                {
                    bCanLoadCutScene = true;
                }
            }
            if(overall_acc_dt > 88.0f)
            {
                bCanLoadCutScene = true;
            }
            if(bCanLoadCutScene)
            {
                LoadLevel("Level01");
            }
        }

        public override void LateUpdate(float dt)
        {

        }

        public override void OnTriggerEnter(uint id)
        {
        }

        public override void OnTriggerExit(uint id)
        {

        }

        void Phase_CS_TP(float start , float end, Vector3 Pos)
        {
            if (overall_acc_dt >= start && overall_acc_dt < end)
            {

                float difference = end - start;
                float section_dt = overall_acc_dt - start;
                float split_time = section_dt * difference;
                if (split_time >= 1.0f)
                    split_time = 1.0f;
                ECS.SetGlobalPosition(CS_Board, Vector3.Lerp(ECS.GetGlobalPosition(CS_Board), Pos, split_time));
            }
        }

        //Note FOV can exist together with POS in the same timeline
        void Phase_Cam_FOV(float start, float end, float old_fov , float fov)
        {
            if (overall_acc_dt < end && overall_acc_dt >= start)
            {

                float difference = end - start;
                float section_dt = overall_acc_dt - start;
                float split_time = section_dt / difference;
                if (split_time >= 1.0f)
                    split_time = 1.0f;
                ECS.SetFOV(entityID, PPMath.Lerp(old_fov, fov, split_time));
            }
        }

        void Phase_Cam_TP_FOV(float start, float end, float old_fov, float fov, float speed = 5.0f)
        {
            if (overall_acc_dt < end && overall_acc_dt >= start)
            {

                float section_dt = overall_acc_dt - start;
                float split_time = speed * section_dt;
                if (split_time >= 1.0f)
                    split_time = 1.0f;
                ECS.SetFOV(entityID, PPMath.Lerp(old_fov, fov, split_time));
            }
        }


        void Phase_Cam_POS(float start, float end, Vector3 old_pos, Vector3 Pos)
        {
            if (overall_acc_dt < end && overall_acc_dt >= start)
            {

                float difference = end - start;
                float section_dt = overall_acc_dt - start;
                float split_time = section_dt / difference;
                if (split_time >= 1.0f)
                    split_time = 1.0f;
                ECS.SetGlobalPosition(entityID, Vector3.Lerp(old_pos, Pos, split_time));
            }
        }

        void Phase_Cam_TP_POS(float start, float end, Vector3 old_pos, Vector3 Pos, float speed = 5.0f)
        {
            if (overall_acc_dt < end && overall_acc_dt >= start)
            {
                float section_dt = overall_acc_dt - start;
                float split_time =  speed * section_dt;
                if (split_time >= 1.0f)
                    split_time = 1.0f;
                ECS.SetGlobalPosition(entityID, Vector3.Lerp(old_pos, Pos, split_time));
            }
        }

        void LoadLevel(string LevelName)
        {
            GameUtilities.LoadScene(LevelName);
        }

    }
}
