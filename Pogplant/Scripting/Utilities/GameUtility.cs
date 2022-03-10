using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.CompilerServices;

namespace Scripting
{
    class GameUtilities
    {
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void GetMousePos(ref float _X, ref float _Y);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static int CheckBounds(Vector3 _Position, Vector3 _Velocity);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static uint Instantiate(string name, Vector3 _Position, Vector3 _Rotation);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static uint InstantiateParticle(string name, Vector3 _Position, Vector3 _Rotation, bool _Parented = false, uint _parentID = 0);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void FollowPlayerCam(uint playerCam, uint playerBox, uint playerShip, Vector3 _Position, Vector3 _Rotation, float _deltaTime);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        //Get Global _Position for player bullet and Global _Rotation, use forwardvector to shoot
        public extern static void FirePlayerBullet(Vector3 _Position, Vector3 _ForwardVector, Vector3 _Rotation, bool homing, uint tracker);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void SpawnStaticExplosion(Vector3 _Position, int type);


        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void FireEnemyBullet(uint entityID, Vector3 _Position, Vector3 _Rotation, float _Speed, float _Lifetime, bool isTrue = false);
        //[MethodImplAttribute(MethodImplOptions.InternalCall)]
        //public extern static void DebugSphere(float pos_x, float pos_y, float pos_z, float camdir_x, float camdir_y, float camdit_z, float _Radius);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void UpdatePlayerHealth_UI();

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void PlayerTakeDamage(uint Player_ID, float _Damage, uint dashboardID, uint face);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static bool GetAlive(uint entityID);
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void PlayEnemyDeathAnimation(uint entityID);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void StartMissile(uint entityID);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void RunMissilePhase1(uint entityID, uint IndicatorID , ref bool blink_phase1,
        ref float large_blink_scale, ref float small_blink_scale, ref float accumulated_scale, ref float Scale_duration,
        ref bool start_blinking, ref float blink_phase_dt, ref float accu_dt_blink, ref bool isBig, ref bool missle_drop_phase2,
        float dt);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void RunMissilePhase2(uint m_DropMissile,  ref bool set_missle_start,
        ref float missile_scale, ref Vector3 Start_drop_pos, ref Vector3 End_drop_pos,
        ref float accu_dt_drop, ref float missle_drop_speed, ref bool missle_drop_phase2, ref bool explode_phase3, float dt);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void RunMissilePhase3(uint m_Explosion, ref float final_scale_value, ref bool set_explode_start, ref Vector3 start_scale,
        ref float accu_dt_expand, ref float explosion_expand_multiplier, ref float centre_shift_multiplier, ref float extends_multiplier_Y, ref float extends_multiplier_XZ,
        ref float scale_down_dt,ref float scale_down_time,ref bool explode_phase3, ref bool m_End, ref Vector3 start_centre, ref Vector3 start_extends, float dt);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void SetTurretFire(uint entityID, bool isActivated, int TurretIdentifier);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void UpdateScore(uint text_object, uint score);

        //[MethodImplAttribute(MethodImplOptions.InternalCall)]
        //public extern static void IncreaseScorefromEnv(uint entityID);

        //[MethodImplAttribute(MethodImplOptions.InternalCall)]
        //public extern static void UpdateDashboardFace(uint entityID, uint face);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void StartLaser(uint entityID);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void InvokeScriptFunction(uint entityID, string _script_name, string _script_function);

        public static GameObject InstantiateObject(string prefabName, Vector3 position, Vector3 rotation, string tag = "")
        {
            var id = Instantiate(prefabName, position, rotation);
            Transform t = ECS.GetComponent<Transform>(id);

            return new GameObject(id, t, tag);
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void LoadScene(string levelToLoad);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void ExitScene();

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void PauseScene();

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void ResumeScene();

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static string GetSceneName();

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void EnemyTakeDamageFromID(uint entityID, float damage);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void UpdateComboUI(uint text_object, uint score);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void UpdateTextColor(uint entityID, Vector3 color);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static uint UpdateScore_AddMinus(uint text_object, uint score, bool isAdd);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static float GetGamma();

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void SetGamma(float newGamma);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void MoveWithImpulse(uint obj_to_move, Vector3 dir, float speed);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static void StopMoving(uint obj_to_move);

        /*************************************************************************/
        /*!
        \brief
            Gets a ray direction from the camera, based off the screen coordinates
            Use the camera position as ray origin, result of this function as
            the ray's direction.

        \param cameraEntityID
            The entity ID of the object containing camera component

        \param screenCoordinates
            The screen coordinate you wanna cast a ray at
        */
        /*************************************************************************/
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public extern static Vector3 GetRayCastDirCamera(uint cameraEntityID, Vector3 screenCoordinates);
    }
}
