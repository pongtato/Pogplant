﻿/******************************************************************************/
/*!
\file	CatmullRomSpline.cs
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
using System.IO;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace Scripting
{
    class CatmullRomSpline
    {

        static private List<Transform> controlPointsList = new List<Transform>();
        private bool isLooping = false;

        private float step_size = 0.2f;

        private List<GameObject> control_points = new List<GameObject>();
        private static bool created_gameobjects = false;

        static bool isInitialized = false;

        public CatmullRomSpline()
        {
        }

        public void InitializeSpline()
        {
            if (!isInitialized)
            {
                //ReadControlPointsFromFile("M3_CP_data.txt");
                ReadControlPointsFromScene("Control_Points");
                isInitialized = true;
                //CopyRotation();
                //ModCPFile();
                //CreateGameObjectsFromCPList();
                //WriteControlPointsToFile();
            }
        }

        // Links a series of splines into chains and returns their waypoints.
        public void DisplayCatmullRomSplineChain()
        {
            if (controlPointsList.Count == 0)
                return;
            // Resulting waypoint of the entire spline chain
            List<Transform> waypoints = new List<Transform>();

            for (int i = 1; i < controlPointsList.Count; i++)
            {
                if ((i == controlPointsList.Count - 1) && !isLooping)
                    continue;

                // Calculate the waypoints for one spline section and add it to the waypoints.
                DisplayCatmullRomSpline(i);
            }
        }



        //Display a spline between 2 points derived with the Catmull-Rom spline algorithm
        void DisplayCatmullRomSpline(int pos)
        {
            //The 4 points we need to form a spline between p1 and p2
            Vector3 p0 = controlPointsList[ClampListPos(pos - 1)].Position;
            Vector3 p1 = controlPointsList[pos].Position;
            Vector3 p2 = controlPointsList[ClampListPos(pos + 1)].Position;
            Vector3 p3 = controlPointsList[ClampListPos(pos + 2)].Position;

            //The start position of the line
            Vector3 lastPos = p1;

            //The spline's resolution
            //Make sure it's is adding up to 1, so 0.3 will give a gap, but 0.2 will work

            //How many times should we loop?
            int loops = (int)(1f / step_size);

            for (int i = 1; i <= loops; i++)
            {
                //Which t position are we at?
                float t = i * step_size;

                //Find the coordinate between the end points with a Catmull-Rom spline
                Vector3 newPos = GetCatmullRomPosition(t, p0, p1, p2, p3);

                //Draw this line segment
                DebugDraw.DebugLine(lastPos, newPos);

                //Save this pos so we can draw the next line segment
                lastPos = newPos;
            }
        }

        // Links a series of splines into chains and returns their waypoints.
        public List<Transform> CalculateCatmullRomChain()
        {
            if (controlPointsList.Count == 0)
                return null;
          
            // Resulting waypoint of the entire spline chain
            List<Transform> waypoints = new List<Transform>();
            int remove_range = (int)(10 / (step_size * 10));
            for (int i = 1; i < controlPointsList.Count; i++)
            {
                if ((i == controlPointsList.Count - 1) && !isLooping)
                {
                    continue;
                }
                // Calculate the waypoints for one spline section 
                // and add it to the waypoints.
                waypoints.AddRange(CalculateCatmullRomSpline(i));
            }
            // Cull the start and end of the waypoint array because they are screwed up
            waypoints.RemoveRange(0, remove_range);

            return waypoints;
        }

        // Given a starting index, this generates a spline section of the entire chain and returns a series of waypoints.
        List<Transform> CalculateCatmullRomSpline(int pos)
        {
            List<Transform> waypoints = new List<Transform>();
            //The 4 points we need to form a spline between p1 and p2
            Vector3 p0 = controlPointsList[ClampListPos(pos - 1)].Position;
            Vector3 p1 = controlPointsList[pos].Position;
            Vector3 p2 = controlPointsList[ClampListPos(pos + 1)].Position;
            Vector3 p3 = controlPointsList[ClampListPos(pos + 2)].Position;

            //The spline's resolution
            //Make sure it's is adding up to 1, so 0.3 will give a gap, but 0.2 will work

            //How many times should we loop?
            int loops = (int)(1f / step_size);

            for (int i = 1; i <= loops; i++)
            {
                //Which t position are we at?
                float t = i * step_size;

                //Find the coordinate between the end points with a Catmull-Rom spline
                Vector3 newPos = GetCatmullRomPosition(t, p0, p1, p2, p3);

                waypoints.Add(new Transform(newPos, controlPointsList[pos].Rotation, Vector3.One()));
            }
            return waypoints;
        }

        //Clamp the list positions to allow looping
        int ClampListPos(int pos)
        {
            if (pos < 0)
            {
                pos = controlPointsList.Count - 1;
            }

            if (pos > controlPointsList.Count)
            {
                pos = 1;
            }
            else if (pos > controlPointsList.Count - 1)
            {
                pos = 0;
            }

            return pos;
        }

        //Returns a position between 4 Vector3 with Catmull-Rom spline algorithm
        //http://www.iquilezles.org/www/articles/minispline/minispline.htm
        Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            //The coefficients of the cubic polynomial (except the 0.5f * which I added later for performance)
            Vector3 a = 2f * p1;
            Vector3 b = p2 - p0;
            //Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
            Vector3 c = (2f * p0) - (5f * p1) + (4f * p2) - p3;
            //Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;
            Vector3 d = -p0 + (3f * p1) - (3f * p2) + p3;

            //The cubic polynomial: a + b * t + c * t^2 + d * t^3
            Vector3 pos = 0.5f * (a + (b * t) + (c * (t * t)) + (d * (t * t * t)));

            return pos;
        }

        // Use this function to create gameobjects from the Control Point List
        // NOTE: Use after controlPointList has been initialized and populated
        public void CreateGameObjectsFromCPList()
        {
            if (!created_gameobjects)
            {
                Transform parent_transform = new Transform(Vector3.Zero(), Vector3.Zero(), Vector3.One());
                GameObject parent = ECS.CreateEntity("Control_points_parent", parent_transform);

                //Transform parent_transform = new Transform(Vector3.Zero(), new Vector3(0.0f, 180.0f, 0.0f), Vector3.One() * 0.1f);
                //parent.AddComponent<Renderer>(new Renderer("sphere"));

                int counter = 0; // counter to increment gameobject name
                foreach (Transform cp in controlPointsList)
                {
                    GameObject result = ECS.CreateChild(parent.id, "ControlPoint_" + counter++, cp);
                    result.AddComponent<Renderer>(new Renderer("sphere"));
                    control_points.Add(result);
                }
                created_gameobjects = true;
            }
        }

        // This function converts a Vector3 to string
        string Vector3ToString(Vector3 position)
        {
            string result = null;
            result += position.X.ToString("0.000") + " ";
            result += position.Y.ToString("0.000") + " ";
            result += position.Z.ToString("0.000");

            return result;
        }

        // This function converts a string to a transform component
        Transform StringToTransform(string data)
        {
            Transform transform = new Transform(Vector3.Zero(), Vector3.Zero(), Vector3.One());
            string[] result = data.Split(' ');

            Vector3 offset = new Vector3(0, 20.0f, 0);

            float scale = .1f;

            transform.Position.X = (float.Parse(result[0]) + offset.X) * scale;
            transform.Position.Y = (float.Parse(result[1]) + offset.Y) * scale;
            transform.Position.Z = -(float.Parse(result[2]) + offset.Z) * scale;

            transform.Rotation.X = 0.0f;
            transform.Rotation.Y = 180.0f - float.Parse(result[4]);
            transform.Rotation.Z = 0.0f;
            return transform;
        }

        // Write control points transform to file
        public void WriteControlPointsToFile()
        {
            string filePath = Directory.GetCurrentDirectory() + "\\Resources\\" + "Transform_CP_4" + ".txt";

            // Create a file to write to.
            string text = "";
            for (int i = 0; i < control_points.Count; ++i)
            {
                Console.WriteLine(control_points[i].id);
                Vector3 gpos = ECS.GetGlobalPosition(control_points[i].id);
                Vector3 grot = ECS.GetGlobalRotation(control_points[i].id);
                string line = Vector3ToString(gpos) + " " + Vector3ToString(grot) + '\n';
                text += line;
                //Console.WriteLine(gpos.X + ", " + gpos.Y + ", " + gpos.Z);
            }
            File.WriteAllText(filePath, text);
        }

        // Read control points data from file into control points list
        void ReadControlPointsFromFile(string filePath)
        {
            string abs_path = "Resources\\" + filePath;

            //Console.WriteLine(abs_path);

            string[] lines = System.IO.File.ReadAllLines(abs_path);

            foreach (string data in lines)
            {
                controlPointsList.Add(StringToTransform(data));
            }

            //Console.WriteLine(controlPointsList.Count);
        }

        // This function compares the tags, the tags must contain 1 uint value
        // example: tag = "69"
        public static int TagComparer(uint x, uint y)
        {
            // Get
            uint x_tag = uint.Parse(ECS.GetComponent<Tag>(x).tag);
            uint y_tag = uint.Parse(ECS.GetComponent<Tag>(y).tag);
            if (x_tag == y_tag)
                return 0;
            else if (x_tag > y_tag)
                return 1;
            else
                return -1;
        }

        // Read control point objects from scene into control points list
        void ReadControlPointsFromScene(string controlPointGroupName)
        {
            // Find the parent object that groups up all the control points
            uint control_point_group_id = ECS.FindEntityWithName(controlPointGroupName);

            if (!ECS.CheckValidEntity(control_point_group_id))
            {
                Console.WriteLine("ERROR: Could not find Control_Points object in the scene, \nfalling back to reading control points data from file.");
                ReadControlPointsFromFile("M3_CP_data.txt");
                return;
            }

            //Console.WriteLine("Found control_points id: " + control_point_group_id);

            // Get all the control point in the group as a list

            // convert uint[] to list<int> for sorting
            uint[] temp_list = ECS.GetChildren(control_point_group_id);
            List<uint> control_points_id = new List<uint>();
            foreach (var item in temp_list)
                control_points_id.Add(item);
            //Console.WriteLine("Number of control points in control_point_group is: " + control_points_id.Count);

            control_points_id.Sort(TagComparer); // Sort the list according to their tags OPTIONAL

            // Add each control points' transform into controlPointList
            foreach (var controlPoint in control_points_id)
            {
                controlPointsList.Add(ECS.GetComponent<Transform>(controlPoint));
            }

            Console.WriteLine("Number of control points is: " + controlPointsList.Count);
        }

        // Use this function to modify/transform the CP data file
        void ModCPFile()
        {
            string filename = "Transform_CP_3.txt";

            string[] old_data = File.ReadAllLines("Resources\\" + filename);
            string[] new_lines = new string[old_data.Length];

            // Goes through all control point data and zeroes out the x,z rotation values
            for (int i = 0; i < old_data.Length; ++i)
            {
                string[] old_values = old_data[i].Split(' ');

                string new_line = old_values[0] + " ";
                new_line += old_values[1] + " ";
                new_line += old_values[2] + " ";

                new_line += "0.00" + " ";
                new_line += old_values[4] + " ";
                new_line += "0.00";

                new_lines[i] = new_line;
            }

            File.WriteAllLines("Resources\\" + filename, new_lines);
        }

        // Use this function to copy rotation data from one data file to another
        void CopyRotation()
        {
            string[] old_data = File.ReadAllLines("Resources\\Transform_CPs_2.txt");
            string[] new_data = File.ReadAllLines("Resources\\M3_CP_Data.txt");

            string[] new_lines = new string[new_data.Length];

            for (int i = 0; i < new_data.Length; ++i)
            {
                string[] old_values = old_data[i].Split(' ');
                string[] new_values = new_data[i].Split(' ');

                string new_line = new_values[0] + " ";
                new_line += new_values[1] + " ";
                new_line += new_values[2] + " ";

                new_line += old_values[3] + " ";
                new_line += old_values[4] + " ";
                new_line += old_values[5];

                new_lines[i] = new_line;
            }

            File.WriteAllLines("Resources\\Transform_CP_3.txt", new_lines);
        }
    }
}
