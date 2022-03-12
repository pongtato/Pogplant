using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scripting.Utilities;

namespace Scripting
{
    public class CutScene2Controller : MonoBehaviour
    {

        float overall_acc_dt = 0.0f;
        uint CS_Board = 0;
        Vector3 InitialCanvasPos = new Vector3(0, 0, 0);
        Vector3 InitialCamPos = new Vector3(-0.35f, 0.6f, 3.5f);
        float InitialFOV = 2.4f;
        float HeldDownAccu = 0f;

        bool bIsInputHeldDown = false;
        bool bCanLoadCutScene = false;


        public CutScene2Controller()
        {

        }

        public override void Start()
        {

        }

        public override void Init(ref uint _entityID)
        {
            //Cam is the self id
            entityID = _entityID;
            CS_Board = ECS.FindEntityWithName("CS_Canvas");


            ECS.SetGlobalPosition(CS_Board, InitialCanvasPos);
            ECS.SetGlobalPosition(entityID, InitialCamPos);
            ECS.SetFOV(entityID, InitialFOV);
            ECS.PlayAudio(entityID, 0);
        }

        public override void Update(float dt)
        {
            if (InputUtility.onKeyHeld("RIGHTCLICK"))
            {
                bIsInputHeldDown = true;
            }
            else
            {
                bIsInputHeldDown = false;
                HeldDownAccu = 0;
            }

            overall_acc_dt += dt;
            Phase_Cam_POS_Speed(0, 12.0f, InitialCamPos, new Vector3(-0.35f, 0.28f, 3.5f), 1.0f);
            Phase_Cam_POS(12.0f, 14.3f, new Vector3(-0.35f, 0.28f, 3.5f), new Vector3(-0.33f, 0.29f, 3.5f));

            //Voice mod
            Phase_Cam_TP_POS(14.3f, 14.5f, new Vector3(-0.32f, 0.27f, 3.5f), new Vector3(-0.12f, 0.36f, 4.0f));
            Phase_Cam_POS(14.5f, 16.0f, new Vector3(-0.12f, 0.36f, 4.0f), new Vector3(-0.11f, 0.365f, 4.0f));
            Phase_Cam_POS(16.0f, 19.0f, new Vector3(-0.11f, 0.365f, 4.0f), new Vector3(-0.138f, 0.345f, 4.0f));

            Phase_Cam_POS(19.0f, 22.0f, new Vector3(-0.138f, 0.345f, 4.0f), new Vector3(-0.13f, 0.365f, 4.0f));
            Phase_Cam_POS(22.0f, 27.0f, new Vector3(-0.13f, 0.365f, 4.0f), new Vector3(0.154f, 0.44f, 2.83f));
            Phase_Cam_POS(27.0f, 30.0f, new Vector3(0.154f, 0.44f, 2.83f), new Vector3(0.122f, 0.29f, 2.83f));
            Phase_Cam_POS(30.0f, 31.0f, new Vector3(0.122f, 0.29f, 2.83f), new Vector3(0.122f, 0.3f, 3.45f));
            Phase_Cam_POS(31.0f, 34.0f, new Vector3(0.122f, 0.3f, 3.45f), new Vector3(0.139f, 0.334f, 4.3f));
            Phase_Cam_POS(34.0f, 37.0f, new Vector3(0.139f, 0.334f, 4.3f), new Vector3(0.304f, 0.389f, 4.1f));
            Phase_Cam_POS(37.0f, 39.0f, new Vector3(0.304f, 0.389f, 4.1f), new Vector3(0.358f, 0.31f, 3.8f));
            Phase_Cam_POS(39.0f, 40.0f, new Vector3(0.358f, 0.31f, 3.8f), new Vector3(0.382f, 0.302f, 3.25f));
            Phase_Cam_POS(40.4f, 40.8f, new Vector3(0.382f, 0.302f, 3.25f), new Vector3(0.382f, 0.302f, 3.25f));
            
            //Smack
            Phase_Cam_TP_POS(40.8f, 42.0f, new Vector3(0.382f, 0.302f, 3.25f), new Vector3(0.386f, 0.147f, 3.25f));
            Phase_Cam_POS(41.5f, 50.0f, new Vector3(0.386f, 0.147f, 3.25f), new Vector3(0.11f, 0.094f, 6.19f));
            //Phase_Cam_POS(50.0f, 53.0f, new Vector3(0.11f, 0.094f, 6.19f), new Vector3(0.11f, 0.094f, 6.19f));
            Phase_Cam_TP_POS(53.5f, 54.0f, new Vector3(0.11f, 0.094f, 6.19f), new Vector3(-0.187f, 0.098f, 4.9f));
            //Phase_Cam_TP_POS(54.0f, 56.0f, new Vector3(-0.187f, 0.098f, 4.9f), new Vector3(-0.187f, 0.098f, 4.9f));
            Phase_Cam_POS(56.0f, 59.0f, new Vector3(-0.187f, 0.098f, 4.9f), new Vector3(-0.346f, 0.015f, 3.75f));
            Phase_Cam_POS(59.0f, 61.0f, new Vector3(-0.346f, 0.015f, 3.75f), new Vector3(-0.35f, -0.143f, 3.75f));
            Phase_Cam_TP_POS(63.4f, 64.0f, new Vector3(-0.35f, -0.143f, 3.75f), new Vector3(-0.35f, -0.281f, 4.0f));
            Phase_Cam_POS(66.5f, 70.0f, new Vector3(-0.35f, -0.281f, 4.0f), new Vector3(-0.157f, -0.336f, 3.7f));
            Phase_Cam_POS(70.0f, 72.0f, new Vector3(-0.157f, -0.336f, 3.7f), new Vector3(-0.173f, -0.359f, 3.4f));
            Phase_Cam_POS(72.0f, 73.0f, new Vector3(-0.173f, -0.359f, 3.4f), new Vector3(-0.095f, -0.276f, 3.6f));
            Phase_Cam_TP_POS(74.0f, 74.7f, new Vector3(-0.095f, -0.276f, 3.6f), new Vector3(-0.108f, -0.169f, 3.1f));
            Phase_Cam_POS(74.7f, 77.5f, new Vector3(-0.108f, -0.169f, 3.1f), new Vector3(-0.091f, -0.138f, 4.0f));

            //Energy depleted
            Phase_Cam_TP_POS(77.5f, 78.0f, new Vector3(-0.091f, -0.138f, 4.0f), new Vector3(0.074f, -0.197f, 3.45f));
            Phase_Cam_POS(79.0f, 88.0f, new Vector3(0.074f, -0.197f, 3.45f), new Vector3(0.264f, -0.201f, 6.3f));
            Phase_Cam_POS(90.0f, 100.0f, new Vector3(0.264f, -0.201f, 6.3f), new Vector3(0.264f, -0.65f, 6.3f));
            //Phase_CS_TP(0.9f, 1.3f, new Vector3(0, 0, 0));

            //Phase_Cam_FOV(1.3f, 6.0f, 2.4f, 2.2f);
            //Phase_Cam_POS(6.0f, 16.0f, InitialCamPos, new Vector3(0.114f, 0.38f, 5));
            //Phase_Cam_POS(16.0f, 19.0f, new Vector3(0.114f, 0.38f, 5), new Vector3(0.114f, 0.385f, 5));
            ////Meteor crash into backyard at 17s with woosh sound
            //Phase_Cam_TP_FOV(18.5f, 20.0f, 2.2f, 1.9f, 5);
            //Phase_Cam_TP_POS(18.5f, 20.0f, new Vector3(0.114f, 0.385f, 5), new Vector3(0.34f, 0.375f, 5), 5);

            //Phase_Cam_FOV(20.0f, 25.5f, 1.9f, 1.7f);
            //Phase_Cam_POS(20.0f, 25.5f, new Vector3(0.34f, 0.375f, 5), new Vector3(0.365f, 0.115f, 5));

            //Phase_Cam_FOV(29.0f, 34.0f, 1.7f, 1.4f);
            //Phase_Cam_POS(29.0f, 34.0f, new Vector3(0.365f, 0.115f, 5), new Vector3(0.155f, 0.158f, 5));
            ////Enemy portrait shown at 38s with woosh sound
            //Phase_Cam_TP_FOV(38.5f, 40.0f, 1.4f, 2.35f, 5);
            //Phase_Cam_TP_POS(38.5f, 40.0f, new Vector3(0.155f, 0.158f, 5), new Vector3(-0.07f, 0.126f, 5), 5);


            //Phase_Cam_FOV(44.0f, 45.0f, 2.35f, 1.6f);
            //Phase_Cam_POS(44.0f, 45.0f, new Vector3(-0.07f, 0.126f, 5), new Vector3(-0.32f, 0.19f, 5));
            //Phase_Cam_FOV(45.0f, 47.0f, 1.6f, 1.4f);

            //Phase_Cam_POS(48.0f, 54.0f, new Vector3(-0.32f, 0.19f, 5), new Vector3(-0.225f, -0.08f, 5));
            //Phase_Cam_FOV(48.0f, 54.0f, 1.4f, 1.63f);
            ////Scientist pointing shown at 54s with woosh sound
            //Phase_Cam_TP_POS(54.5f, 55.0f, new Vector3(-0.225f, -0.08f, 5), new Vector3(-0.375f, -0.170f, 5), 5);
            //Phase_Cam_TP_FOV(54.5f, 55.0f, 1.63f, 1.6f, 5);

            //Phase_Cam_POS(55.0f, 57.0f, new Vector3(-0.375f, -0.170f, 5), new Vector3(-0.390f, -0.135f, 5));
            //Phase_Cam_FOV(55.0f, 57.0f, 1.6f, 1.4f);
            ////Scientist pointing shown at 57s with woosh sound
            //Phase_Cam_TP_POS(57.5f, 58.0f, new Vector3(-0.390f, -0.135f, 5), new Vector3(-0.240f, -0.22f, 5), 5);

            //Phase_Cam_POS(58.0f, 59.0f, new Vector3(-0.240f, -0.22f, 5), new Vector3(-0.230f, -0.22f, 5));
            //Phase_Cam_FOV(58.0f, 59.0f, 1.4f, 1.2f);

            //Phase_Cam_POS(59.0f, 60.0f, new Vector3(-0.230f, -0.22f, 5), new Vector3(-0.240f, -0.225f, 5));
            //Phase_Cam_FOV(59.0f, 60.0f, 1.2f, 1.1f);
            ////"Lets get to work" Whoosh sound at 60s
            //Phase_Cam_TP_POS(60.5f, 61.0f, new Vector3(-0.240f, -0.225f, 5), new Vector3(-0.300f, -0.420f, 5));
            //Phase_Cam_TP_FOV(60.5f, 61.0f, 1.1f, 1.6f);

            //Phase_Cam_POS(61f, 62f, new Vector3(-0.300f, -0.420f, 5), new Vector3(-0.310f, -0.412f, 5));
            //Phase_Cam_FOV(61f, 62f, 1.6f, 1.2f);
            ////Music start
            //Phase_Cam_TP_POS(62f, 63f, new Vector3(-0.310f, -0.412f, 5), new Vector3(-0.05f, -0.36f, 5));
            //Phase_Cam_TP_FOV(62f, 63f, 1.2f, 1.6f);

            //Phase_Cam_POS(63f, 64.5f, new Vector3(-0.05f, -0.36f, 5), new Vector3(-0.02f, -0.358f, 5));
            //Phase_Cam_FOV(63f, 64.5f, 1.6f, 1.8f);

            //Phase_Cam_POS(64.5f, 66f, new Vector3(-0.02f, -0.358f, 5), new Vector3(-0.01f, -0.332f, 5));
            //Phase_Cam_FOV(64.5f, 66f, 1.8f, 2.1f);

            //Phase_Cam_POS(66f, 66.7f, new Vector3(-0.01f, -0.332f, 5), new Vector3(0.017f, -0.324f, 5));
            //Phase_Cam_FOV(66f, 66.7f, 2.1f, 2.2f);

            //Phase_Cam_TP_POS(66.7f, 67.3f, new Vector3(0.017f, -0.324f, 5), new Vector3(0.017f, -0.105f, 5));
            //Phase_Cam_TP_FOV(66.7f, 67.3f, 2.2f, 1.8f);

            //Phase_Cam_POS(67.3f, 70f, new Vector3(0.017f, -0.105f, 5), new Vector3(0.017f, -0.08f, 5));
            ////AAAANND DONE part
            //Phase_Cam_TP_POS(70f, 70.8f, new Vector3(0.017f, -0.08f, 5), new Vector3(0.24f, -0.095f, 5));
            //Phase_Cam_TP_FOV(70f, 70.8f, 1.8f, 1.4f);

            //Phase_Cam_TP_POS(71.3f, 72f, new Vector3(0.24f, -0.095f, 5), new Vector3(0.3f, -0.2f, 5));
            //Phase_Cam_TP_FOV(71.3f, 72f, 1.4f, 2.22f);

            //Phase_Cam_POS(72.0f, 79f, new Vector3(0.3f, -0.2f, 5), new Vector3(0.32f, -0.39f, 5));

            //Phase_Cam_POS(81.0f, 82f, new Vector3(0.32f, -0.39f, 5), new Vector3(0.32f, -0.6f, 5));

            if (bIsInputHeldDown)
            {
                HeldDownAccu += dt;
                if (HeldDownAccu > 2.0f)
                {
                    bCanLoadCutScene = true;
                }
            }
            if (overall_acc_dt > 88.0f)
            {
                bCanLoadCutScene = true;
            }
            if (bCanLoadCutScene)
            {
                LoadLevel("Level02");
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

        void Phase_CS_TP(float start, float end, Vector3 Pos)
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
        void Phase_Cam_FOV(float start, float end, float old_fov, float fov)
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

        void Phase_Cam_POS_Speed(float start, float end, Vector3 old_pos, Vector3 Pos, float lerp_speed)
        {
            if (overall_acc_dt < end && overall_acc_dt >= start)
            {
                float difference = end - start;
                float section_dt = overall_acc_dt - start;
                float split_time = section_dt / difference;
                if (split_time >= 1.0f)
                    split_time = 1.0f;

                ECS.SetGlobalPosition(entityID, Vector3.Lerp(old_pos, Pos, split_time * lerp_speed));
            }
        }

        void Phase_Cam_TP_POS(float start, float end, Vector3 old_pos, Vector3 Pos, float speed = 5.0f)
        {
            if (overall_acc_dt < end && overall_acc_dt >= start)
            {
                float section_dt = overall_acc_dt - start;
                float split_time = speed * section_dt;
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
