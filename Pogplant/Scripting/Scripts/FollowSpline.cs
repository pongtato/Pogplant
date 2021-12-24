﻿/******************************************************************************/
/*!
\file	FollowSpline.cs
\author Ng Tian Kiat
\par	email: tiankiat.ng\@digipen.edu
\details
	This script moves a gameobject along a series of waypoints generated by a spline,
    script (Catmullrom).

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
			or disclosure of this file or its contents without the prior
			written consent of DigiPen Institute of Technology is prohibited.
*/
/******************************************************************************/
using System;

namespace Scripting
{
    public class FollowSpline : MonoBehaviour
    {

        public static float follow_speed = 9.0f; // follow speed in m/s
        private int lookat_waypoint_offset = 2; // indicates how many waypoints infront to look at when dealing with rotation

        private Transform[] waypoints = null; // Array of waypoints we get from running the CatmullRomSpline script.
        private float DelayToStart = 0.0f; // delay in seconds until the followspline starts
        private uint current_waypoint_index = 1; // Initiallized to 1 so that skip the first 2 waypoints in waypoints array.
        private float alpha = 0.0f; // Clamped between [0,1], this value is used to interpolate between 2 waypoints.
        private float d_alpha = 0.0f; // This is the rate of change of alpha, determined by the distance between the 2 waypoints and the follow_speed (Essentially the time it takes to go between 2 waypoints)
        private float time_between_waypoint = 0.0f;

        private bool isInit = false;
        private bool isEnd = false; //  This is true when we have arrived a the end of the path
        private CatmullRomSpline catmullRom = null;
        private Transform transform;

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;

            if (ECS.GetTagECS(entityID) == "Player")
                DelayToStart = 1.0f;
            else if (ECS.GetTagECS(entityID) == "WPG_0")
                DelayToStart = 0.0f;
            else if (ECS.GetTagECS(entityID) == "WPG_1")
                DelayToStart = 0.2f;
            else if (ECS.GetTagECS(entityID) == "WPG_2")
                DelayToStart = 0.4f;
            else if (ECS.GetTagECS(entityID) == "WPG_3")
                DelayToStart = 0.6f;
            else if (ECS.GetTagECS(entityID) == "WPG_4")
                DelayToStart = 1.4f;

            Vector3 pos = new Vector3();
            Vector3 rot = new Vector3();
            Vector3 scale = new Vector3();
            ECS.GetTransformECS(entityID, ref pos, ref rot, ref scale);
            transform = new Transform(pos, rot, scale);
        }

        // Start is called before the first frame update
        public override void Start()
        {
            // Initialize waypoint array

            catmullRom = new CatmullRomSpline();
            catmullRom.InitializeSpline();

            var calculated_waypoints = catmullRom.CalculateCatmullRomChain();
            if (calculated_waypoints != null)
            {
                waypoints = catmullRom.CalculateCatmullRomChain().ToArray();
                //Console.WriteLine("Number of waypoints generated: " + waypoints.Length);
            }
            else
            {
                Console.WriteLine("Error: waypoints is null!");
                return;
            }

            // Initialize d_alpha for first waypoint
            // Calculate d_alpha from follow_speed and distance between current and next waypoint
            float distance = Vector3.Distance(waypoints[current_waypoint_index - 1].Position, waypoints[current_waypoint_index].Position);
            d_alpha = 1.0f / (distance / follow_speed);

            //if (ECS.GetTagECS(entityID) == "Player")
            //    catmullRom.DisplayCatmullRomSplineChain();
        }

        // Update is called once per frame
        public override void Update(float dt)
        {
            if (waypoints == null)
                return;

            ECS.GetTransformECS(entityID, ref transform.Position, ref transform.Rotation, ref transform.Scale);
            if (!isInit)
            {
                // this is to initialize the starting position and rotation to the start of the spline
                isInit = true;
                transform.Position = waypoints[0].Position;
                transform.Rotation = waypoints[0].Rotation; // initialize rotation here
                ECS.SetTransformECS(entityID, transform.Position, new Vector3(0, 180, 0), transform.Scale); // NOTE: Rotation is hard coded here because it is imposible to correctly configure rotation of the waypoints from the editor
            }

            if ((DelayToStart -= dt) <= 0.0f)
                FollowWaypoints(dt);

            // Debug the spline
            ECS.SetPosition(entityID, transform.Position);
        }

        public override void LateUpdate(float dt)
        {

        }

        // Use this function to set a new speed for this followspline
        public void UpdateSpeed(float speed)
        {
            follow_speed = speed;
        }

        // This function interpolates the gameobjects transform between the current and next waypoint.
        private void FollowWaypoints(float dt)
        {
            // Only move if we are not at the end of path.
            if (!isEnd)
            {
                float translation_lerpSpeed = 10.0f;

                UpdateCurrentWaypoint(alpha);
                time_between_waypoint += dt;
                alpha = d_alpha * time_between_waypoint; // Calculate the current alpha between the 2 waypoints
                transform.Position += (Vector3.Lerp(waypoints[current_waypoint_index - 1].Position, waypoints[current_waypoint_index].Position, alpha) - transform.Position) * translation_lerpSpeed * dt;
                ECS.SetTransformECS(entityID, transform.Position, transform.Rotation, transform.Scale);

                float rotation_lerp_speed = 1.5f;

                // Rotate the game object by finding a waypoint infront to look at
                Vector3 forward = Transform.GetForwardVector(entityID);
                Vector3 look_direction = waypoints[current_waypoint_index + lookat_waypoint_offset].Position - ECS.GetGlobalPosition(entityID);
                Vector3 look_vector = look_direction - forward;
                float interpolant = rotation_lerp_speed * dt / look_vector.magnitude();
                Vector3 look_point = ECS.GetGlobalPosition(entityID) + (forward + (look_vector * interpolant));

                Transform.LookAtClamped(entityID, look_point);
            }
        }

        // This function updates the current waypoint and recalculates variables used for interpolation,
        // as well as stops the followspline based on the end_offset value.
        void UpdateCurrentWaypoint(float alpha)
        {
            int end_offset = 25;// waypoint_offset + 15;
            if (alpha >= 1.0)
            {
                // Calculate d_alpha from follow_speed and distance between current and next waypoint
                float distance = Vector3.Distance(waypoints[current_waypoint_index].Position, waypoints[++current_waypoint_index].Position);
                d_alpha = 1.0f / (distance / follow_speed);

                time_between_waypoint = 0.0f; // Reset current time between waypoints
            }

            if (current_waypoint_index >= waypoints.Length - end_offset - 1)
                isEnd = true;
        }

        public void SetLockRotation(bool isLock)
        {
            //lockRotation = isLock;
        }

        public override void OnTriggerEnter(uint id)
        {

        }
        public override void OnTriggerExit(uint id)
        {

        }
    }
}
