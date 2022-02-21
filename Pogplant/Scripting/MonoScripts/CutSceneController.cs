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
        Vector3 InitialCamPos = new Vector3(-0.3f, 0.38f, 5);
        float InitialFOV = 2.4f;

        public CutSceneController() 
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
        }

        public override void Update(float dt)
        {
            //For the first 0.5 sec
            overall_acc_dt += dt;
            //Play move up animation
            //if(overall_acc_dt < 0.9f)
            //{
            //    float section_dt = overall_acc_dt;
            //    float split_time = section_dt / 0.9f;
            //    if (split_time >= 1.0f)s
            //        split_time = 1.0f;new Vector3(0, 0.03f, 0)
            //    ECS.SetGlobalPosition(CS_Board, Vector3.Lerp(ECS.GetGlobalPosition(CS_Board), new Vector3(0, 0.03f, 0), split_time));
            //}
            Phase_CS(0, 0.5f, new Vector3(0, 0.03f, 0));
            Phase_CS(0.5f, 0.9f, new Vector3(0, -0.03f, 0));
            Phase_CS(0.9f, 1.3f, new Vector3(0, 0, 0));
            Phase_Cam_FOV(1.3f, 6.0f, 2.4f, 2.3f);
            Phase_Cam_POS(6.0f, 16.0f, InitialCamPos,  new Vector3(0.114f, 0.38f, 5));

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

        void Phase_CS(float start , float end, Vector3 Pos)
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
            if (overall_acc_dt >= start && overall_acc_dt < end)
            {

                float difference = end - start;
                float section_dt = overall_acc_dt - start;
                float split_time = section_dt / difference;
                if (split_time >= 1.0f)
                    split_time = 1.0f;
                ECS.SetFOV(entityID, PPMath.Lerp(old_fov, fov, split_time));
            }
        }
        void Phase_Cam_POS(float start, float end, Vector3 old_pos, Vector3 Pos)
        {
            if (overall_acc_dt >= start && overall_acc_dt < end)
            {

                float difference = end - start;
                float section_dt = overall_acc_dt - start;
                float split_time = section_dt / difference;
                //if (split_time >= 1.0f)
                //    split_time = 1.0f;
                ECS.SetGlobalPosition(entityID, Vector3.Lerp(old_pos, Pos, split_time));
            }
        }

    }
}
