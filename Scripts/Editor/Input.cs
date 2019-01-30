
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FVTC.LearningInnovations.Unity.Editor
{

    public class Input
    {



        [MenuItem("Learning Innovations/Input/Add Generic Button and Axes")]
        static void AddGenericAxesAndButtons()
        {
            SerializedObject inputManagerAsset = new SerializedObject(AssetDatabase.LoadAssetAtPath("ProjectSettings/InputManager.asset", typeof(UnityEngine.Object)));


            var existingAxes = GetInputManagerAxisNames(inputManagerAsset);

            foreach(var axis in GetGenericAxes())
            {
                if (!existingAxes.Contains(axis.Name))
                {
                    AddAxis(axis, inputManagerAsset);
                }
            }

            foreach(var button in GetGenericButtons())
            {
                if (!existingAxes.Contains(button.Name))
                {
                    AddAxis(button, inputManagerAsset);
                }
            }

            inputManagerAsset.ApplyModifiedProperties();

        }


        private static void AddAxis(InputManagerAxis axis, SerializedObject inputManagerAsset)
        {
            SerializedProperty axesProperty = inputManagerAsset.FindProperty("m_Axes");

            // Creates a new axis by incrementing the size of the m_Axes array.
            axesProperty.arraySize++;

            // Get the new axis be querying for the last array element.
            SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);

            // Iterate through all the properties of the new axis.
            while (axisProperty.Next(true))
            {
                switch (axisProperty.name)
                {
                    case "m_Name":
                        axisProperty.stringValue = axis.Name;
                        break;
                    case "descriptiveName":
                        axisProperty.stringValue = axis.DescriptiveName;
                        break;
                    case "descriptiveNegativeName":
                        axisProperty.stringValue = axis.DescriptiveNegativeName;
                        break;
                    case "negativeButton":
                        axisProperty.stringValue = axis.NegativeButton;
                        break;
                    case "positiveButton":
                        axisProperty.stringValue = axis.PositiveButton;
                        break;
                    case "altNegativeButton":
                        axisProperty.stringValue = axis.AltNegativeButton;
                        break;
                    case "altPositiveButton":
                        axisProperty.stringValue = axis.AltPositiveButton;
                        break;
                    case "gravity":
                        axisProperty.floatValue = axis.Gravity;
                        break;
                    case "dead":
                        axisProperty.floatValue = axis.Dead;
                        break;
                    case "sensitivity":
                        axisProperty.floatValue = axis.Sensitivity;
                        break;
                    case "snap":
                        axisProperty.boolValue = axis.Snap;
                        break;
                    case "invert":
                        axisProperty.boolValue = axis.Invert;
                        break;
                    case "type":
                        axisProperty.intValue = (int)axis.Type;
                        break;
                    case "axis":
                        axisProperty.intValue = axis.Axis - 1;
                        break;
                    case "joyNum":
                        axisProperty.intValue = axis.JoyNum;
                        break;
                }
            }
        }


        static HashSet<string> GetInputManagerAxisNames(SerializedObject inputManagerAsset)
        {
            SerializedProperty axesProperty = inputManagerAsset.FindProperty("m_Axes");

            HashSet<string> axisNames = new HashSet<string>();

            for (int i = 0; i < axesProperty.arraySize; i++)
            {
                axisNames.Add(axesProperty.GetArrayElementAtIndex(i).displayName);
            }

            return axisNames;
        }
        

        static IEnumerable<InputManagerAxis> GetGenericAxes()
        {
            yield return new InputManagerAxis
            {
                Name = "AXIS_X",
                Dead = 0.19f,
                Sensitivity = 1f,
                Type = AxisType.JoystickAxis,
                Axis = 0,
                Invert = false
            };

            yield return new InputManagerAxis
            {
                Name = "AXIS_Y",
                Dead = 0.19f,
                Sensitivity = 1f,
                Type = AxisType.JoystickAxis,
                Axis = 1,
                Invert = true
            };

            for (int i = 2; i <= 29; ++i)
            {
                yield return new InputManagerAxis
                {
                    Name = string.Format("AXIS_{0}", i),
                    Dead = 0.19f,
                    Sensitivity = 1f,
                    Type = AxisType.JoystickAxis,
                    Axis = i,
                    Invert = true
                };
            }
        }

        static IEnumerable<InputManagerAxis> GetGenericButtons()
        {
            for (int i = 1; i <= 30; ++i)
            {
                yield return new InputManagerAxis
                {
                    Name = string.Format("BUTTON_{0}", i),
                    PositiveButton = string.Format("joystick button {0}", i),
                    Gravity = 1000,
                    Dead = 0.001f,
                    Sensitivity = 100f,
                    Type = AxisType.KeyOrMouseButton
                };
            }
        }

        private class InputManagerAxis
        {
            public string Name = "";
            public string DescriptiveName = "";
            public string DescriptiveNegativeName = "";
            public string NegativeButton = "";
            public string PositiveButton = "";
            public string AltNegativeButton = "";
            public string AltPositiveButton = "";
            public float Gravity = 0.0f;
            public float Dead = 0.0f;
            public float Sensitivity = 0.0f;
            public bool Snap = false;
            public bool Invert = false;
            public AxisType Type = default(AxisType);
            public int Axis = 0;
            public int JoyNum = 0;
        }

        /*
         *     m_Name: CONTROLLER_RIGHT_STICK_CLICK
    descriptiveName: 
    descriptiveNegativeName: 
    negativeButton: 
    positiveButton: joystick button 9
    altNegativeButton: 
    altPositiveButton: 
    gravity: 1000
    dead: 0.001
    sensitivity: 1000
    snap: 0
    invert: 0
    type: 0
    axis: 0
    joyNum: 0


                descriptiveName: 
    descriptiveNegativeName: 
    negativeButton: 
    positiveButton: 
    altNegativeButton: 
    altPositiveButton: 
    gravity: 0
    dead: 0.19
    sensitivity: 1
    snap: 0
    invert: 0
    type: 2
    axis: 6
    joyNum: 0

         * 
         */

        /// <summary>
        /// Used to map AxisType from a useful name to the int value the InputManager wants.
        /// </summary>
        private enum AxisType
        {
            KeyOrMouseButton = 0,
            MouseMovement,
            JoystickAxis
        };

    }
}
