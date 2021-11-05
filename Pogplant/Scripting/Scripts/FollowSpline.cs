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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    public class FollowSpline : MonoBehaviour
    {
        private CatmullRomSpline catmullRom = null;

        private float follow_speed = 44.0f;

        private float rotation_speed = 10.0f;



        private Transform[] waypoints = null; // Array of waypoints we get from running the CatmullRomSpline script.
        private uint current_waypoint_index = 1; // Initiallized to 2 so that skip the first 3 waypoints in waypoints array.
        private float alpha = 0.0f; // Clamped between [0,1], this value is used to interpolate between 2 waypoints.
        private float d_alpha = 0.0f; // This is the rate of change of alpha, determined by the distance between the 2 waypoints and the follow_speed (Essentially the time it takes to go between 2 waypoints)
        private bool isEnd = false; //  This is true when we have arrived a the end of the path
        private float time_between_waypoint = 0.0f;

        private bool lockRotation = false;

        public override void Init(ref uint _entityID)
        {
            entityID = _entityID;
        }

        // Start is called before the first frame update
        public override void Start()
        {
            // Initialize waypoint array

            catmullRom = new CatmullRomSpline();
            catmullRom.InitializeSpline();

            waypoints = catmullRom.CalculateCatmullRomChain().ToArray();
            Console.WriteLine(waypoints.Length);

            //for (int i = 0; i < waypoints.Length; ++i)
            //    Console.WriteLine("waypoint " + i + " is at: x = " + waypoints[i].X + ", y = " + waypoints[i].Y + ", z = " + waypoints[i].Z);

            // Initialize d_alpha for first waypoint
            // Calculate d_alpha from follow_speed and distance between current and next waypoint
            float distance = Vector3.Distance(waypoints[current_waypoint_index - 1].Position, waypoints[current_waypoint_index].Position);
            d_alpha = 1.0f / (distance / follow_speed);

            //Console.WriteLine("alpha is" + d_alpha);

            // Initialize starting position and rotation of the play area

            //start_time = Time.realtimeSinceStartup;
        }

        // Update is called once per frame
        public override void Update(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {
            FollowWaypoints(ref transform, ref dt);

            //temp debug
            //for (int i = 0; i < waypoints.Length; ++i)
            //    DebugDraw.DebugSphere(waypoints[i].X, waypoints[i].Y, waypoints[i].Z, 0.0f, 0.0f, 0.0f, 0.2f);

            for (int i = 0; i < waypoints.Length; ++i)
                DebugDraw.DebugSphere(waypoints[i].Position, Vector3.Zero(), 0.2f, 36);

        }

        public override void LateUpdate(ref Transform transform, ref Rigidbody rigidbody, ref float dt)
        {

        }

        private float lerpSpeed = 5.0f;

        // This function interpolates the gameobjects transform between the current and next waypoint.
        private void FollowWaypoints(ref Transform transform, ref float dt)
        {
            // Only move if we are not at the end of path.
            if (!isEnd)
            {
                UpdateCurrentWaypoint(alpha);
                time_between_waypoint += dt;
                alpha = d_alpha * time_between_waypoint; // Calculate the current alpha between the 2 waypoints
                transform.Position += (Vector3.Lerp(waypoints[current_waypoint_index - 1].Position, waypoints[current_waypoint_index].Position, alpha) - transform.Position) * lerpSpeed * dt;
                //transform.Position = Vector3.MoveTowards(transform.Position, waypoints[current_waypoint_index], follow_speed * dt);
                //if (transform.Position != waypoints[current_waypoint_index])
                //    transform.Rotation = Quaternion.Slerp(transform.Rotation, Quaternion.LookRotation(waypoints[current_waypoint_index + 1] - transform.Position), rotation_speed * Time.deltaTime);

                //if (lockRotation)
                //{
                    transform.Rotation = Vector3.Lerp(transform.Rotation, waypoints[current_waypoint_index + 1].Rotation, lerpSpeed * dt);
                //}
            }
        }
        void UpdateCurrentWaypoint(float alpha)
        {
            if (alpha >= 1.0)
            {
                // Calculate d_alpha from follow_speed and distance between current and next waypoint
                float distance = Vector3.Distance(waypoints[current_waypoint_index].Position, waypoints[++current_waypoint_index].Position);
                d_alpha = 1.0f / (distance / follow_speed);

                time_between_waypoint = 0.0f; // Reset current time between waypoints
            }

            if (current_waypoint_index == waypoints.Length - 2)
                isEnd = true;
        }

        public void SetLockRotation (bool isLock)
        {
            lockRotation = isLock;
        }

        public void LateUpdate(ref Transform transform, ref Rigidbody rigidbody)
        {
        }
    }
}
