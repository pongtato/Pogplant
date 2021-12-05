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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripting
{
    class CatmullRomSpline
    {
        public List<Transform> controlPointsList = new List<Transform>();
        public List<GameObject> control_points = new List<GameObject>();
        public bool isLooping = false;

        public float step_size = 0.2f;

        static bool created_gameobjects = false;
        public CatmullRomSpline()
        {
        }

        public void InitializeSpline()
        {
            //ReadControlPointsFromFile("Control_Points_Curved.txt");
            ReadControlPointsFromFile("M3_CP_data.txt");
            //CopyRotation();
            //ModCPFile();
            //CreateGameObjectsFromCPList();
            //WriteControlPointsToFile();
            //Console.WriteLine(controlPointsList.Count);
        }

        void ModCPFile()
        {
            string[] old_data = File.ReadAllLines("Resources\\Transform_CP_3.txt");

            string[] new_lines = new string[old_data.Length];

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

            File.WriteAllLines("Resources\\Transform_CP_3.txt", new_lines);
        }

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

        public void CreateGameObjectsFromCPList()
        {
            if (!created_gameobjects)
            {
                //Transform parent_transform = new Transform(Vector3.Zero(), new Vector3(0.0f, 180.0f, 0.0f), Vector3.One() * 0.1f);
                Transform parent_transform = new Transform(Vector3.Zero(), Vector3.Zero(), Vector3.One());
                GameObject parent = ECS.CreateEntity("Control_points_parent", parent_transform);
                //parent.AddComponent<Renderer>(new Renderer("sphere"));
            
                int counter = 0;
                foreach (Transform cp in controlPointsList)
                {
                    GameObject result = ECS.CreateChild(parent.id, "ControlPoint_" + counter++, cp);
                    result.AddComponent<Renderer>(new Renderer("sphere"));
                    control_points.Add(result);
                    //Console.WriteLine(result.id);
                }
                created_gameobjects = true;
            }
        }

        // Links a series of splines into chains and returns their waypoints.
        public void DisplayCatmullRomSplineChain()
        {
            //Console.WriteLine("CalculateChain called");
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
                //Gizmos.DrawLine(lastPos, newPos);
                DebugDraw.DebugLine(lastPos, newPos);

                //Save this pos so we can draw the next line segment
                lastPos = newPos;
            }
        }

        // Links a series of splines into chains and returns their waypoints.
        public List<Transform> CalculateCatmullRomChain()
        {
            //Console.WriteLine("CalculateChain called");
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
            //for (int i = 0; i < remove_range; ++i)
            //	waypoints.RemoveAt(waypoints.Count);
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

            //Console.WriteLine("p " + 0 + " is at: x = " + p0.X + ", y = " + p0.Y + ", z = " + p0.Z);
            //Console.WriteLine("p " + 1 + " is at: x = " + p1.X + ", y = " + p1.Y + ", z = " + p1.Z);
            //Console.WriteLine("p " + 2 + " is at: x = " + p2.X + ", y = " + p2.Y + ", z = " + p2.Z);
            //Console.WriteLine("p " + 3 + " is at: x = " + p3.X + ", y = " + p3.Y + ", z = " + p3.Z);

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

                //Console.WriteLine("waypoint " + i + " is at: x = " + newPos.X + ", y = " + newPos.Y + ", z = " + newPos.Z);

                waypoints.Add(new Transform(newPos, controlPointsList[pos].Rotation, Vector3.One()));

                //Save this pos so we can draw the next line segment
                lastPos = newPos;
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
            //Console.WriteLine("p " + 0 + " is at: x = " + p0.X + ", y = " + p0.Y + ", z = " + p0.Z);
            //Console.WriteLine("p " + 1 + " is at: x = " + p1.X + ", y = " + p1.Y + ", z = " + p1.Z);
            //Console.WriteLine("p " + 2 + " is at: x = " + p2.X + ", y = " + p2.Y + ", z = " + p2.Z);
            //Console.WriteLine("p " + 3 + " is at: x = " + p3.X + ", y = " + p3.Y + ", z = " + p3.Z);

            Vector3 a = 2f * p1;
            Vector3 b = p2 - p0;
            //Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
            Vector3 c = (2f * p0) - (5f * p1) + (4f * p2) - p3;
            //Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;
            Vector3 d = -p0 + (3f * p1) - (3f * p2) + p3;

            //Console.WriteLine("a "  + " is at: x = " + a.X + ", y = " + a.Y + ", z = " + a.Z);
            //Console.WriteLine("b "  + " is at: x = " + b.X + ", y = " + b.Y + ", z = " + b.Z);
            //Console.WriteLine("c "  + " is at: x = " + c.X + ", y = " + c.Y + ", z = " + c.Z);
            //Console.WriteLine("d "  + " is at: x = " + d.X + ", y = " + d.Y + ", z = " + d.Z);

            //The cubic polynomial: a + b * t + c * t^2 + d * t^3
            //Vector3 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * (t * t * t)));

            Vector3 pos = 0.5f * (a + (b * t) + (c * (t * t)) + (d * (t * t * t)));

            return pos;
        }

        string Vector3ToString(Vector3 position)
        {
            string result = null;
            result += position.X.ToString("0.000") + " ";
            result += position.Y.ToString("0.000") + " ";
            result += position.Z.ToString("0.000");

            return result;
        }

        Transform StringToTransform(string data)
        {
            Transform transform = new Transform(Vector3.Zero(), Vector3.Zero(), Vector3.One());
            string[] result = data.Split(' ');

            Vector3 offset = new Vector3(0, 20.0f, 0);

            float scale = .1f;

            transform.Position.X = (float.Parse(result[0]) + offset.X) * scale;
            transform.Position.Y = (float.Parse(result[1]) + offset.Y) * scale;
            transform.Position.Z = -(float.Parse(result[2]) + offset.Z) * scale;

            transform.Rotation.X = 0.0f;// float.Parse(result[3]);
            transform.Rotation.Y = 180.0f - float.Parse(result[4]);
            transform.Rotation.Z = 0.0f;// float.Parse(result[5]);
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

        // Write control points transform to file
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
    }
}
