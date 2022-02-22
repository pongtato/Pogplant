using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class MovingParts
    {
        public uint entity_id;

        //For ECS get transform
        Vector3 pos = new Vector3();
        Vector3 rot = new Vector3();
        Vector3 scale = new Vector3();

        //Position
        float lerp_position_speed_X;
        float lerp_position_speed_Y;
        float lerp_position_speed_Z;

        Vector3 lerp_position_upper_limit;
        Vector3 lerp_linear_position;
        Vector3 lerp_position_lower_limit;

        bool lerp_position_positive_direction_X;       //+ve means going towards upper limit, -ve going towards lower limit
        bool lerp_position_positive_direction_Y;
        bool lerp_position_positive_direction_Z;

        bool lerp_position_ping_pong_X;
        bool lerp_position_ping_pong_Y;
        bool lerp_position_ping_pong_Z;

        bool update_pingpong_position = false;

        bool lerp_position_linear_X;
        bool lerp_position_linear_Y;
        bool lerp_position_linear_Z;

        bool update_linear_position = false;

        //Rotation
        float lerp_rotation_speed_X;
        float lerp_rotation_speed_Y;
        float lerp_rotation_speed_Z;

        Vector3 lerp_rotation_upper_limit;
        Vector3 lerp_linear_rotation;
        Vector3 lerp_rotation_lower_limit;

        bool lerp_rotation_positive_direction_X;
        bool lerp_rotation_positive_direction_Y;
        bool lerp_rotation_positive_direction_Z;

        bool lerp_rotation_ping_pong_X;
        bool lerp_rotation_ping_pong_Y;
        bool lerp_rotation_ping_pong_Z;

        bool toggle_spin;
        Vector3 spin_rotation;

        bool update_pingpong_rotation = false;

        bool lerp_rotation_linear_X;
        bool lerp_rotation_linear_Y;
        bool lerp_rotation_linear_Z;

        bool update_linear_rotation = false;

        //Scale
        float lerp_scale_speed_X;
        float lerp_scale_speed_Y;
        float lerp_scale_speed_Z;

        Vector3 lerp_scale_upper_limit;
        Vector3 lerp_scale_lower_limit;

        bool lerp_scale_positive_direction_X;
        bool lerp_scale_positive_direction_Y;
        bool lerp_scale_positive_direction_Z;

        bool lerp_scale_ping_pong_X;
        bool lerp_scale_ping_pong_Y;
        bool lerp_scale_ping_pong_Z;

        bool update_scale = false;

        //Position
        void SetLerpPosSpeedX(float value)
        {
            lerp_position_speed_X = value;
        }

        void SetLerpPosSpeedY(float value)
        {
            lerp_position_speed_Y = value;
        }

        void SetLerpPosSpeedZ(float value)
        {
            lerp_position_speed_Z = value;
        }

        void SetLerpPosUpperLimit(Vector3 value)
        {
            lerp_position_upper_limit = value;
        }

        void SetLerpPosLowerLimit(Vector3 value)
        {
            lerp_position_lower_limit = value;
        }

        void SetLerpLinearPosition(Vector3 value)
        {
            lerp_linear_position = value;
        }

        void SetLerpPosPositiveDirectionX(bool value)
        {
            lerp_position_positive_direction_X = value;
        }

        void SetLerpPosPositiveDirectionY(bool value)
        {
            lerp_position_positive_direction_Y = value;
        }

        void SetLerpPosPositiveDirectionZ(bool value)
        {
            lerp_position_positive_direction_Z = value;
        }

        void SetLerpPosPingPongX(bool value)
        {
            lerp_position_ping_pong_X = value;
        }

        void SetLerpPosPingPongY(bool value)
        {
            lerp_position_ping_pong_Y = value;
        }

        void SetLerpPosPingPongZ(bool value)
        {
            lerp_position_ping_pong_Z = value;
        }

        void SetUpdatePingPongPosition(bool value)
        {
            update_pingpong_position = value;
        }

        void SetLerpPosLinearX(bool value)
        {
            lerp_position_linear_X = value;
        }

        void SetLerpPosLinearY(bool value)
        {
            lerp_position_linear_Y = value;
        }

        void SetLerpPosLinearZ(bool value)
        {
            lerp_position_linear_Z = value;
        }

        void SetUpdateLinearPosition(bool value)
        {
            update_linear_position = value;
        }

        //Rotation
        void SetLerpRotSpeedX(float value)
        {
            lerp_rotation_speed_X = value;
        }

        void SetLerpRotSpeedY(float value)
        {
            lerp_rotation_speed_Y = value;
        }

        void SetLerpRotSpeedZ(float value)
        {
            lerp_rotation_speed_Z = value;
        }

        void SetLerpRotUpperLimit(Vector3 value)
        {
            lerp_rotation_upper_limit = value;
        }

        void SetLerpRotLowerLimit(Vector3 value)
        {
            lerp_rotation_lower_limit = value;
        }

        void SetLerpRotPositiveDirectionX(bool value)
        {
            lerp_rotation_positive_direction_X = value;
        }

        void SetLerpRotPositiveDirectionY(bool value)
        {
            lerp_rotation_positive_direction_Y = value;
        }

        void SetLerpRotPositiveDirectionZ(bool value)
        {
            lerp_rotation_positive_direction_Z = value;
        }

        void SetLerpRotPingPongX(bool value)
        {
            lerp_rotation_ping_pong_X = value;
        }

        void SetLerpRotPingPongY(bool value)
        {
            lerp_rotation_ping_pong_Y = value;
        }

        void SetLerpRotPingPongZ(bool value)
        {
            lerp_rotation_ping_pong_Z = value;
        }

        void SetUpdatePingPongRotation(bool value)
        {
            update_pingpong_rotation = value;
        }

        void SetUpdateLinearRotation(bool value)
        {
            update_linear_rotation = value;
        }

        void SetLerpRotLinearX(bool value)
        {
            lerp_rotation_linear_X = value;
        }

        void SetLerpRotLinearY(bool value)
        {
            lerp_rotation_linear_Y = value;
        }

        void SetLerpRotLinearZ(bool value)
        {
            lerp_rotation_linear_Z = value;
        }

        void SetLerpLinearRotation(Vector3 value)
        {
            lerp_linear_rotation = value;
        }

        //Scale
        void SetLerpScaleSpeedX(float value)
        {
            lerp_scale_speed_X = value;
        }

        void SetLerpScaleSpeedY(float value)
        {
            lerp_scale_speed_Y = value;
        }

        void SetLerpScaleSpeedZ(float value)
        {
            lerp_scale_speed_Z = value;
        }

        void SetLerpScaleUpperLimit(Vector3 value)
        {
            lerp_scale_upper_limit = value;
        }

        void SetLerpScaleLowerLimit(Vector3 value)
        {
            lerp_scale_lower_limit = value;
        }

        void SetLerpScalePositiveDirectionX(bool value)
        {
            lerp_scale_positive_direction_X = value;
        }

        void SetLerpScalePositiveDirectionY(bool value)
        {
            lerp_scale_positive_direction_Y = value;
        }

        void SetLerpScalePositiveDirectionZ(bool value)
        {
            lerp_scale_positive_direction_Z = value;
        }

        void SetLerpScalePingPongX(bool value)
        {
            lerp_scale_ping_pong_X = value;
        }

        void SetLerpScalePingPongY(bool value)
        {
            lerp_scale_ping_pong_Y = value;
        }

        void SetLerpScalePingPongZ(bool value)
        {
            lerp_scale_ping_pong_Z = value;
        }

        public void SetUpdateScale(bool value)
        {
            update_scale = value;
        }

        //Misc
        public void SetToggleSpin(bool value)
        {
            toggle_spin = value;
        }

        //Reset all
        public void ResetAllVariables()
        {
            lerp_position_speed_X = 0.0f;
            lerp_position_speed_Y = 0.0f;
            lerp_position_speed_Z = 0.0f;

            lerp_position_upper_limit = new Vector3();
            lerp_linear_position = new Vector3();
            lerp_position_lower_limit = new Vector3();

            lerp_position_positive_direction_X = false;       //+ve means going towards upper limit, -ve going towards lower limit
            lerp_position_positive_direction_Y = false;
            lerp_position_positive_direction_Z = false;

            lerp_position_ping_pong_X = false;
            lerp_position_ping_pong_Y = false;
            lerp_position_ping_pong_Z = false;

            update_pingpong_position = false;

            lerp_position_linear_X = false;
            lerp_position_linear_Y = false;
            lerp_position_linear_Z = false;

            update_linear_position = false;

            //Rotation
            lerp_rotation_speed_X = 0.0f;
            lerp_rotation_speed_Y = 0.0f;
            lerp_rotation_speed_Z = 0.0f;

            lerp_rotation_upper_limit = new Vector3();
            lerp_linear_rotation = new Vector3();
            lerp_rotation_lower_limit = new Vector3();

            lerp_rotation_positive_direction_X = false;
            lerp_rotation_positive_direction_Y = false;
            lerp_rotation_positive_direction_Z = false;

            lerp_rotation_ping_pong_X = false;
            lerp_rotation_ping_pong_Y = false;
            lerp_rotation_ping_pong_Z = false;

            toggle_spin = false;
            spin_rotation = new Vector3();

            update_pingpong_rotation = false;

            lerp_rotation_linear_X = false;
            lerp_rotation_linear_Y = false;
            lerp_rotation_linear_Z = false;

            update_linear_rotation = false;

            //Scale
            lerp_scale_speed_X = 0.0f;
            lerp_scale_speed_Y = 0.0f;
            lerp_scale_speed_Z = 0.0f;

            lerp_scale_upper_limit = new Vector3();
            lerp_scale_lower_limit = new Vector3();

            lerp_scale_positive_direction_X = false;
            lerp_scale_positive_direction_Y = false;
            lerp_scale_positive_direction_Z = false;

            lerp_scale_ping_pong_X = false;
            lerp_scale_ping_pong_Y = false;
            lerp_scale_ping_pong_Z = false;

            update_scale = false;
        }

        #region[Set & Update moving parts animation]

        public void SetPingPongPosition(Vector3 set_pos_lower_limit, Vector3 set_pos_upper_limit, Vector3 lerp_speed,
            bool set_positive_direction_X, bool set_positive_direction_Y, bool set_positive_direction_Z,
            bool set_ping_pong_X, bool set_ping_pong_Y, bool set_ping_pong_Z)
        {
            //Update the moving part's parameters
            SetLerpPosSpeedX(lerp_speed.X);
            SetLerpPosSpeedY(lerp_speed.Y);
            SetLerpPosSpeedZ(lerp_speed.Z);

            SetLerpPosUpperLimit(set_pos_upper_limit);
            SetLerpPosLowerLimit(set_pos_lower_limit);

            SetLerpPosPingPongX(set_ping_pong_X);
            SetLerpPosPingPongY(set_ping_pong_Y);
            SetLerpPosPingPongZ(set_ping_pong_Z);

            SetLerpPosPositiveDirectionX(set_positive_direction_X);
            SetLerpPosPositiveDirectionY(set_positive_direction_Y);
            SetLerpPosPositiveDirectionZ(set_positive_direction_Z);

            //Automatically enable since already setting
            SetUpdatePingPongPosition(true);
        }

        public void SetLinearPosition(Vector3 set_target_pos, Vector3 lerp_speed, bool set_lerp_X, bool set_lerp_Y, bool set_lerp_Z)
        {
            SetLerpLinearPosition(set_target_pos);

            SetLerpPosSpeedX(lerp_speed.X);
            SetLerpPosSpeedY(lerp_speed.Y);
            SetLerpPosSpeedZ(lerp_speed.Z);

            SetLerpPosLinearX(set_lerp_X);
            SetLerpPosLinearY(set_lerp_Y);
            SetLerpPosLinearZ(set_lerp_Z);

            SetUpdateLinearPosition(true);
        }

        public void SetPingPongRotation(Vector3 set_rot_lower_limit, Vector3 set_rot_upper_limit, Vector3 lerp_speed,
            bool set_positive_direction_X, bool set_positive_direction_Y, bool set_positive_direction_Z,
            bool set_ping_pong_X, bool set_ping_pong_Y, bool set_ping_pong_Z)
        {
            //Update the moving part's parameters
            SetLerpRotSpeedX(lerp_speed.X);
            SetLerpRotSpeedY(lerp_speed.Y);
            SetLerpRotSpeedZ(lerp_speed.Z);

            SetLerpRotUpperLimit(set_rot_upper_limit);
            SetLerpRotLowerLimit(set_rot_lower_limit);

            SetLerpRotPingPongX(set_ping_pong_X);
            SetLerpRotPingPongY(set_ping_pong_Y);
            SetLerpRotPingPongZ(set_ping_pong_Z);

            SetLerpRotPositiveDirectionX(set_positive_direction_X);
            SetLerpRotPositiveDirectionY(set_positive_direction_Y);
            SetLerpRotPositiveDirectionZ(set_positive_direction_Z);

            //Automatically enable since already setting
            SetUpdatePingPongRotation(true);
        }

        public void SetLinearRotation(Vector3 set_target_rot, Vector3 lerp_speed, bool set_lerp_X, bool set_lerp_Y, bool set_lerp_Z)
        {
            SetLerpLinearRotation(set_target_rot);

            SetLerpRotSpeedX(lerp_speed.X);
            SetLerpRotSpeedY(lerp_speed.Y);
            SetLerpRotSpeedZ(lerp_speed.Z);


            SetLerpRotLinearX(set_lerp_X);
            SetLerpRotLinearY(set_lerp_Y);
            SetLerpRotLinearZ(set_lerp_Z);

            SetUpdateLinearRotation(true);
        }

        public void SetMovingPartsScale(Vector3 set_scale_lower_limit, Vector3 set_scale_upper_limit, Vector3 lerp_speed,
            bool set_positive_direction_X, bool set_positive_direction_Y, bool set_positive_direction_Z,
            bool set_ping_pong_X, bool set_ping_pong_Y, bool set_ping_pong_Z)
        {
            //Update the moving part's parameters
            SetLerpScaleSpeedX(lerp_speed.X);
            SetLerpScaleSpeedY(lerp_speed.Y);
            SetLerpScaleSpeedZ(lerp_speed.Z);

            SetLerpScaleUpperLimit(set_scale_upper_limit);
            SetLerpScaleLowerLimit(set_scale_lower_limit);

            SetLerpScalePingPongX(set_ping_pong_X);
            SetLerpScalePingPongY(set_ping_pong_Y);
            SetLerpScalePingPongZ(set_ping_pong_Z);

            SetLerpScalePositiveDirectionX(set_positive_direction_X);
            SetLerpScalePositiveDirectionY(set_positive_direction_Y);
            SetLerpScalePositiveDirectionZ(set_positive_direction_Z);

            //Automatically enable since already setting
            SetUpdateScale(true);
        }

        public void UpdateMovingParts(float dt)
        {
            //Update linear position only if needed
            if (update_linear_position)
            {
                UpdateLinearPosition(dt);
            }
            //Update pingpong position only if needed
            if (update_pingpong_position)
            {
                UpdatePingPongPosition(dt);
            }

            //Update rotation only if needed
            if (update_pingpong_rotation)
            {
                UpdatePingPongRotation(dt);
            }
            //Update linear rotation only if needed
            if (update_linear_rotation)
            {
                UpdateLinearRotation(dt);
            }

            //Update scale only if needed
            if (update_scale)
            {
                UpdateMovingPartsScale(dt);
            }
        }

        void UpdatePingPongPosition(float dt)
        {
            //===============================================================================================================================================//
            //X Axis
            //===============================================================================================================================================//

            //Keep playing if ping pong
            if (lerp_position_ping_pong_X)
            {
                ECS.GetTransformECS(entity_id, ref pos, ref rot, ref scale);

                //Move 
                if (lerp_position_positive_direction_X)
                {
                    ECS.SetPosition(entity_id, Vector3.Lerp(pos, new Vector3(lerp_position_upper_limit.X, pos.Y, pos.Z), lerp_position_speed_X * dt));

                    //Check for limit and reverse
                    if (pos.X >= lerp_position_upper_limit.X - 0.1f)
                    {
                        lerp_position_positive_direction_X = false;
                    }
                }
                else
                {
                    ECS.SetPosition(entity_id, Vector3.Lerp(pos, new Vector3(lerp_position_lower_limit.X, pos.Y, pos.Z), lerp_position_speed_X * dt));

                    if (pos.X <= lerp_position_lower_limit.X + 0.1f)
                    {
                        lerp_position_positive_direction_X = true;
                    }
                }
            }
            //Play anim once if no ping pong
            else
            {
                ECS.GetTransformECS(entity_id, ref pos, ref rot, ref scale);

                //Move 
                if (lerp_position_positive_direction_X)
                {
                    ECS.SetPosition(entity_id, Vector3.Lerp(pos, new Vector3(lerp_position_upper_limit.X, pos.Y, pos.Z), lerp_position_speed_X * dt));
                }
                else
                {
                    ECS.SetPosition(entity_id, Vector3.Lerp(pos, new Vector3(lerp_position_lower_limit.X, pos.Y, pos.Z), lerp_position_speed_X * dt));
                }
            }

            //===============================================================================================================================================//
            //Y Axis
            //===============================================================================================================================================//

            //Keep playing if ping pong
            if (lerp_position_ping_pong_Y)
            {
                ECS.GetTransformECS(entity_id, ref pos, ref rot, ref scale);

                //Move 
                if (lerp_position_positive_direction_Y)
                {
                    ECS.SetPosition(entity_id, Vector3.Lerp(pos, new Vector3(pos.X, lerp_position_upper_limit.Y, pos.Z), lerp_position_speed_Y * dt));

                    //Check for limit and reverse
                    if (pos.Y >= lerp_position_upper_limit.Y - 0.1f)
                    {
                        lerp_position_positive_direction_Y = false;
                    }
                }
                else
                {
                    ECS.SetPosition(entity_id, Vector3.Lerp(pos, new Vector3(pos.X, lerp_position_lower_limit.Y, pos.Z), lerp_position_speed_Y * dt));

                    if (pos.Y <= lerp_position_lower_limit.Y + 0.1f)
                    {
                        lerp_position_positive_direction_Y = true;
                    }
                }
            }
            //Play anim once if no ping pong
            else
            {
                ECS.GetTransformECS(entity_id, ref pos, ref rot, ref scale);

                //Move 
                if (lerp_position_positive_direction_Y)
                {
                    ECS.SetPosition(entity_id, Vector3.Lerp(pos, new Vector3(pos.X, lerp_position_upper_limit.Y, pos.Z), lerp_position_speed_Y * dt));
                }
                else
                {
                    ECS.SetPosition(entity_id, Vector3.Lerp(pos, new Vector3(pos.X, lerp_position_lower_limit.Y, pos.Z), lerp_position_speed_Y * dt));
                }
            }

            //===============================================================================================================================================//
            //Z Axis
            //===============================================================================================================================================//

            //Keep playing if ping pong
            if (lerp_position_ping_pong_Z)
            {
                ECS.GetTransformECS(entity_id, ref pos, ref rot, ref scale);

                //Move 
                if (lerp_position_positive_direction_Z)
                {
                    ECS.SetPosition(entity_id, Vector3.Lerp(pos, new Vector3(pos.X, pos.Y, lerp_position_upper_limit.Z), lerp_position_speed_Z * dt));

                    //Check for limit and reverse
                    if (pos.Z >= lerp_position_upper_limit.Z - 0.1f)
                    {
                        lerp_position_positive_direction_Z = false;
                    }
                }
                else
                {
                    ECS.SetPosition(entity_id, Vector3.Lerp(pos, new Vector3(pos.X, pos.Y, lerp_position_lower_limit.Z), lerp_position_speed_Z * dt));

                    if (pos.Z <= lerp_position_lower_limit.Z + 0.1f)
                    {
                        lerp_position_positive_direction_Z = true;
                    }
                }
            }
            //Play anim once if no ping pong
            else
            {
                ECS.GetTransformECS(entity_id, ref pos, ref rot, ref scale);

                //Move 
                if (lerp_position_positive_direction_Z)
                {
                    ECS.SetPosition(entity_id, Vector3.Lerp(pos, new Vector3(pos.X, pos.Y, lerp_position_upper_limit.Z), lerp_position_speed_Z * dt));
                }
                else
                {
                    ECS.SetPosition(entity_id, Vector3.Lerp(pos, new Vector3(pos.X, pos.Y, lerp_position_lower_limit.Z), lerp_position_speed_Z * dt));
                }
            }
        }

        void UpdateLinearPosition(float dt)
        {
            if (lerp_position_linear_X)
            {
                ECS.GetTransformECS(entity_id, ref pos, ref rot, ref scale);
                ECS.SetPosition(entity_id, Vector3.Lerp(pos, new Vector3(lerp_linear_position.X, pos.Y, pos.Z), lerp_position_speed_X * dt));
            }
            if (lerp_position_linear_Y)
            {
                ECS.GetTransformECS(entity_id, ref pos, ref rot, ref scale);
                ECS.SetPosition(entity_id, Vector3.Lerp(pos, new Vector3(pos.X, lerp_linear_position.Y, pos.Z), lerp_position_speed_Y * dt));
            }
            if (lerp_position_linear_Z)
            {
                ECS.GetTransformECS(entity_id, ref pos, ref rot, ref scale);
                ECS.SetPosition(entity_id, Vector3.Lerp(pos, new Vector3(pos.X, pos.Y, lerp_linear_position.Z), lerp_position_speed_Z * dt));
            }
        }

        void UpdatePingPongRotation(float dt)
        {
            //===============================================================================================================================================//
            //X Axis
            //===============================================================================================================================================//

            //Keep playing if ping pong
            if (lerp_rotation_ping_pong_X)
            {
                ECS.GetTransformECS(entity_id, ref pos, ref rot, ref scale);

                //Rotate 
                if (lerp_rotation_positive_direction_X)
                {


                    ECS.SetRotation(entity_id, Vector3.Lerp(rot, new Vector3(lerp_rotation_upper_limit.X, rot.Y, rot.Z), lerp_rotation_speed_X * dt));

                    //Check for limit and reverse
                    if (rot.X >= lerp_rotation_upper_limit.X - 0.1f)
                    {
                        lerp_rotation_positive_direction_X = false;
                    }
                }
                else
                {
                    ECS.SetRotation(entity_id, Vector3.Lerp(rot, new Vector3(lerp_rotation_lower_limit.X, rot.Y, rot.Z), lerp_rotation_speed_X * dt));

                    if (rot.X <= lerp_rotation_lower_limit.X + 0.1f)
                    {
                        lerp_rotation_positive_direction_X = true;
                    }
                }
            }
            //Play anim once if no ping pong
            else
            {
                ECS.GetTransformECS(entity_id, ref pos, ref rot, ref scale);

                //Rotate 
                if (lerp_rotation_positive_direction_X)
                {
                    ECS.SetRotation(entity_id, Vector3.Lerp(rot, new Vector3(lerp_rotation_upper_limit.X, rot.Y, rot.Z), lerp_rotation_speed_X * dt));
                }
                else
                {
                    ECS.SetRotation(entity_id, Vector3.Lerp(rot, new Vector3(lerp_rotation_lower_limit.X, rot.Y, rot.Z), lerp_rotation_speed_X * dt));
                }
            }

            //===============================================================================================================================================//
            //Y Axis
            //===============================================================================================================================================//

            //Keep playing if ping pong
            if (lerp_rotation_ping_pong_Y)
            {
                ECS.GetTransformECS(entity_id, ref pos, ref rot, ref scale);

                //Rotate 
                if (lerp_rotation_positive_direction_Y)
                {
                    ECS.SetRotation(entity_id, Vector3.Lerp(rot, new Vector3(rot.X, lerp_rotation_upper_limit.Y, rot.Z), lerp_rotation_speed_Y * dt));

                    //Check for limit and reverse
                    if (rot.Y >= lerp_rotation_upper_limit.Y - 0.1f)
                    {
                        lerp_rotation_positive_direction_Y = false;
                    }
                }
                else
                {
                    ECS.SetRotation(entity_id, Vector3.Lerp(rot, new Vector3(rot.X, lerp_rotation_lower_limit.Y, rot.Z), lerp_rotation_speed_Y * dt));

                    if (rot.Y <= lerp_rotation_lower_limit.Y + 0.1f)
                    {
                        lerp_rotation_positive_direction_Y = true;
                    }
                }
            }
            //Play anim once if no ping pong
            else
            {
                ECS.GetTransformECS(entity_id, ref pos, ref rot, ref scale);

                //Rotate 
                if (lerp_rotation_positive_direction_Y)
                {
                    ECS.SetRotation(entity_id, Vector3.Lerp(rot, new Vector3(rot.X, lerp_rotation_upper_limit.Y, rot.Z), lerp_rotation_speed_Y * dt));
                }
                else
                {
                    ECS.SetRotation(entity_id, Vector3.Lerp(rot, new Vector3(rot.X, lerp_rotation_lower_limit.Y, rot.Z), lerp_rotation_speed_Y * dt));
                }
            }

            //===============================================================================================================================================//
            //Z Axis
            //===============================================================================================================================================//

            //Keep playing if ping pong
            if (lerp_rotation_ping_pong_Z)
            {
                ECS.GetTransformECS(entity_id, ref pos, ref rot, ref scale);

                //Rotate 
                if (lerp_rotation_positive_direction_Z)
                {
                    ECS.SetRotation(entity_id, Vector3.Lerp(rot, new Vector3(rot.X, rot.Y, lerp_rotation_upper_limit.Z), lerp_rotation_speed_Z * dt));

                    //Check for limit and reverse
                    if (rot.Z >= lerp_rotation_upper_limit.Z - 0.1f)
                    {
                        lerp_rotation_positive_direction_Z = false;
                    }
                }
                else
                {
                    ECS.SetRotation(entity_id, Vector3.Lerp(rot, new Vector3(rot.X, rot.Y, lerp_rotation_lower_limit.Z), lerp_rotation_speed_Z * dt));

                    if (rot.Z <= lerp_rotation_lower_limit.Z + 0.1f)
                    {
                        lerp_rotation_positive_direction_Z = true;
                    }
                }
            }
            //Play anim once if no ping pong
            else
            {
                ECS.GetTransformECS(entity_id, ref pos, ref rot, ref scale);

                //Rotate 
                if (lerp_rotation_positive_direction_Z)
                {
                    ECS.SetRotation(entity_id, Vector3.Lerp(rot, new Vector3(rot.X, rot.Y, lerp_rotation_upper_limit.Z), lerp_rotation_speed_Z * dt));
                }
                else
                {
                    ECS.SetRotation(entity_id, Vector3.Lerp(rot, new Vector3(rot.X, rot.Y, lerp_rotation_lower_limit.Z), lerp_rotation_speed_Z * dt));
                }
            }
        }

        void UpdateLinearRotation(float dt)
        {
            if (lerp_rotation_linear_X)
            {
                ECS.GetTransformECS(entity_id, ref pos, ref rot, ref scale);
                ECS.SetRotation(entity_id, Vector3.Lerp(rot, new Vector3(lerp_linear_rotation.X, rot.Y, rot.Z), lerp_rotation_speed_X * dt));
            }
            if (lerp_rotation_linear_Y)
            {
                ECS.GetTransformECS(entity_id, ref pos, ref rot, ref scale);
                ECS.SetRotation(entity_id, Vector3.Lerp(rot, new Vector3(rot.X, lerp_linear_rotation.Y, rot.Z), lerp_rotation_speed_Y * dt));
            }
            if (lerp_rotation_linear_Z)
            {
                ECS.GetTransformECS(entity_id, ref pos, ref rot, ref scale);
                ECS.SetRotation(entity_id, Vector3.Lerp(rot, new Vector3(rot.X, rot.Y, lerp_linear_rotation.Z), lerp_rotation_speed_Z * dt));
            }
        }

        void UpdateMovingPartsScale(float dt)
        {
            //===============================================================================================================================================//
            //X Axis
            //===============================================================================================================================================//

            //Keep playing if ping pong
            if (lerp_scale_ping_pong_X)
            {
                ECS.GetTransformECS(entity_id, ref pos, ref scale, ref scale);

                //Rotate 
                if (lerp_scale_positive_direction_X)
                {
                    ECS.SetScale(entity_id, Vector3.Lerp(scale, new Vector3(lerp_scale_upper_limit.X, scale.Y, scale.Z), lerp_scale_speed_X * dt));

                    //Check for limit and reverse
                    if (scale.X >= lerp_scale_upper_limit.X - 0.1f)
                    {
                        lerp_scale_positive_direction_X = false;
                    }
                }
                else
                {
                    ECS.SetScale(entity_id, Vector3.Lerp(scale, new Vector3(lerp_scale_lower_limit.X, scale.Y, scale.Z), lerp_scale_speed_X * dt));

                    if (scale.X <= lerp_scale_lower_limit.X + 0.1f)
                    {
                        lerp_scale_positive_direction_X = true;
                    }
                }
            }
            //Play anim once if no ping pong
            else
            {
                ECS.GetTransformECS(entity_id, ref pos, ref scale, ref scale);

                //Rotate 
                if (lerp_scale_positive_direction_X)
                {
                    ECS.SetScale(entity_id, Vector3.Lerp(scale, new Vector3(lerp_scale_upper_limit.X, scale.Y, scale.Z), lerp_scale_speed_X * dt));
                }
                else
                {
                    ECS.SetScale(entity_id, Vector3.Lerp(scale, new Vector3(lerp_scale_lower_limit.X, scale.Y, scale.Z), lerp_scale_speed_X * dt));
                }
            }

            //===============================================================================================================================================//
            //Y Axis
            //===============================================================================================================================================//

            //Keep playing if ping pong
            if (lerp_scale_ping_pong_Y)
            {
                ECS.GetTransformECS(entity_id, ref pos, ref scale, ref scale);

                //Rotate 
                if (lerp_scale_positive_direction_Y)
                {
                    ECS.SetScale(entity_id, Vector3.Lerp(scale, new Vector3(scale.X, lerp_scale_upper_limit.Y, scale.Z), lerp_scale_speed_Y * dt));

                    //Check for limit and reverse
                    if (scale.Y >= lerp_scale_upper_limit.Y - 0.1f)
                    {
                        lerp_scale_positive_direction_Y = false;
                    }
                }
                else
                {
                    ECS.SetScale(entity_id, Vector3.Lerp(scale, new Vector3(scale.X, lerp_scale_lower_limit.Y, scale.Z), lerp_scale_speed_Y * dt));

                    if (scale.Y <= lerp_scale_lower_limit.Y + 0.1f)
                    {
                        lerp_scale_positive_direction_Y = true;
                    }
                }
            }
            //Play anim once if no ping pong
            else
            {
                ECS.GetTransformECS(entity_id, ref pos, ref scale, ref scale);

                //Rotate 
                if (lerp_scale_positive_direction_Y)
                {
                    ECS.SetScale(entity_id, Vector3.Lerp(scale, new Vector3(scale.X, lerp_scale_upper_limit.Y, scale.Z), lerp_scale_speed_Y * dt));
                }
                else
                {
                    ECS.SetScale(entity_id, Vector3.Lerp(scale, new Vector3(scale.X, lerp_scale_lower_limit.Y, scale.Z), lerp_scale_speed_Y * dt));
                }
            }

            //===============================================================================================================================================//
            //Z Axis
            //===============================================================================================================================================//

            //Keep playing if ping pong
            if (lerp_scale_ping_pong_Z)
            {
                ECS.GetTransformECS(entity_id, ref pos, ref scale, ref scale);

                //Rotate 
                if (lerp_scale_positive_direction_Z)
                {
                    ECS.SetScale(entity_id, Vector3.Lerp(scale, new Vector3(scale.X, scale.Y, lerp_scale_upper_limit.Z), lerp_scale_speed_Z * dt));

                    //Check for limit and reverse
                    if (scale.Z >= lerp_scale_upper_limit.Z - 0.1f)
                    {
                        lerp_scale_positive_direction_Z = false;
                    }
                }
                else
                {
                    ECS.SetScale(entity_id, Vector3.Lerp(scale, new Vector3(scale.X, scale.Y, lerp_scale_lower_limit.Z), lerp_scale_speed_Z * dt));

                    if (scale.Z <= lerp_scale_lower_limit.Z + 0.1f)
                    {
                        lerp_scale_positive_direction_Z = true;
                    }
                }
            }
            //Play anim once if no ping pong
            else
            {
                ECS.GetTransformECS(entity_id, ref pos, ref scale, ref scale);

                //Rotate 
                if (lerp_scale_positive_direction_Z)
                {
                    ECS.SetScale(entity_id, Vector3.Lerp(scale, new Vector3(scale.X, scale.Y, lerp_scale_upper_limit.Z), lerp_scale_speed_Z * dt));
                }
                else
                {
                    ECS.SetScale(entity_id, Vector3.Lerp(scale, new Vector3(scale.X, scale.Y, lerp_scale_lower_limit.Z), lerp_scale_speed_Z * dt));
                }
            }
        }

        public void SpinObjectEndless(float x_axis, float y_axis, float z_axis, float spin_speed, float dt)
        {
            if (toggle_spin)
            {
                spin_rotation.X += x_axis * spin_speed * dt;
                spin_rotation.Y += y_axis * spin_speed * dt;
                spin_rotation.Z += z_axis * spin_speed * dt;

                ClampRotationValue(ref spin_rotation.X, -360.0f, 360.0f);
                ClampRotationValue(ref spin_rotation.Y, -360.0f, 360.0f);
                ClampRotationValue(ref spin_rotation.Z, -360.0f, 360.0f);

                ECS.SetRotation(entity_id, spin_rotation);
            }
        }

        public float CheckNearest(float value, float comp1, float comp2)
        {
            return Math.Abs(comp1 - value) > Math.Abs(comp2 - value) ? comp2 : comp1;
        }

        void ClampRotationValue(ref float value, float min, float max)
        {
            if (value <= min)
                value = 0.0f;
            else if (value >= max)
                value = 0.0f;
        }

        public void StopObjectSpin()
        {
            toggle_spin = false;
        }
        #endregion
    }
}
