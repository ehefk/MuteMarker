using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace MuteMarker.Scripts.Editor
{
    public static class MuteMarkerAnimTool
    {
        private static int FindParameterIndex(AnimatorController source, string name)
        {
            for (int i = 0; i < source.parameters.Length; i++)
            {
                if (source.parameters[i].name.Equals(name))
                    return i;
            }

            return -1;
        }

        private static AnimatorControllerLayer FindLayer(AnimatorController source, string name)
        {
            foreach (AnimatorControllerLayer item in source.layers)
            {
                if (item.name.Equals(name))
                {
                    return item;
                }
            }

            return null;
        }

        private static int FindLayerIndex(AnimatorController source, string name)
        {
            for (int i = 0; i < source.layers.Length; i++)
            {
                if (source.layers[i].name.Equals(name))
                {
                    return i;
                }
            }

            return -1;
        }

        private static AnimatorState FindState(AnimatorStateMachine target, AnimatorState source)
        {
            if (!source || !target) return null;

            return FindState(target, source.name);
        }

        private static AnimatorState FindState(AnimatorStateMachine target, string source)
        {
            if (!target) return null;

            foreach (var t in target.states)
            {
                if (t.state.name.Equals(source))
                {
                    return t.state;
                }
            }

            return null;
        }

        private static AnimatorStateMachine FindStateStateMachine(AnimatorStateMachine target, AnimatorStateMachine source)
        {
            if (!source || !target) return null;

            foreach (var t in target.stateMachines)
            {
                if (t.stateMachine.name.Equals(source.name))
                {
                    return t.stateMachine;
                }
            }

            return null;
        }

        private static void CopyLayerWithData(AnimatorController target, AnimatorControllerLayer source, bool overwrite)
        {
            // Duplicate stateMachine.
            AnimatorStateMachine stateMachine = source.stateMachine;

            // Delete duplicate layers
            if (overwrite)
            {
                int index = FindLayerIndex(target, source.name);
                if (index != -1) target.RemoveLayer(index);
            }

            // Instantiate new StateMachine
            AnimatorStateMachine newStateMachine = new AnimatorStateMachine
            {
                name = stateMachine.name,
                hideFlags = stateMachine.hideFlags
            };

            AssetDatabase.AddObjectToAsset(newStateMachine, target);

            // Copy State
            foreach (ChildAnimatorState childState in stateMachine.states)
            {
                AnimatorState state = childState.state;

                AnimatorState newState = CopyNewState(state);
                newState.behaviours = CopyBehaviours(target, state.behaviours);

                newStateMachine.AddState(newState, childState.position);
                AssetDatabase.AddObjectToAsset(newState, target);
            }

            newStateMachine.anyStateTransitions = CopyNewTransitions(target, newStateMachine, stateMachine.anyStateTransitions);
            newStateMachine.anyStatePosition = stateMachine.anyStatePosition;

            newStateMachine.entryPosition = stateMachine.entryPosition;
            newStateMachine.defaultState = FindState(newStateMachine, stateMachine.defaultState);

            newStateMachine.exitPosition = stateMachine.exitPosition;

            for (int j = 0; j < stateMachine.states.Length; j++)
            {
                newStateMachine.states[j].state.transitions = CopyNewTransitions(target, newStateMachine, stateMachine.states[j].state.transitions);
            }

            // Copy Layer
            string name = target.MakeUniqueLayerName(source.name);
            AnimatorControllerLayer newLayer = CopyNewLayer(source, newStateMachine, true, name);
            target.AddLayer(newLayer);
        }
        
        public static void CopyLayerWithData(AnimatorController target, AnimatorController source, string name,
            bool overwrite)
        {
            AnimatorControllerLayer layer = FindLayer(source, name);

            CopyLayerWithData(target, layer, overwrite);
        }

        private static AnimatorStateTransition[] CopyNewTransitions(AnimatorController controller, AnimatorStateMachine stateMachine, AnimatorStateTransition[] source)
        {
            List<AnimatorStateTransition> transitionList = new List<AnimatorStateTransition>();
            foreach (AnimatorStateTransition transition in source)
            {
                AnimatorStateTransition newTransition = CopyNewTransition(stateMachine, transition);
                foreach (AnimatorCondition condition in transition.conditions)
                {
                    newTransition.AddCondition(condition.mode, condition.threshold, condition.parameter);
                }

                AssetDatabase.AddObjectToAsset(newTransition, controller);
                transitionList.Add(newTransition);
            }

            return transitionList.ToArray();
        }

        private static StateMachineBehaviour[] CopyBehaviours(AnimatorController controller, StateMachineBehaviour[] source)
        {
            List<StateMachineBehaviour> list = new List<StateMachineBehaviour>();
            foreach (StateMachineBehaviour item in source)
            {
                StateMachineBehaviour newBehaviour = CopyBehaviour(item);
                AssetDatabase.AddObjectToAsset(newBehaviour, controller);
                list.Add(newBehaviour);
            }

            return list.ToArray();
        }
        
        public static void RemoveLayer(AnimatorController target, string name)
        {
            int index = FindLayerIndex(target, name);
            if (index != -1) target.RemoveLayer(index);
        }

        private static AnimatorControllerLayer CopyNewLayer(AnimatorControllerLayer source, AnimatorStateMachine stateMachine, bool isFirst, string name)
        {
            AnimatorControllerLayer layer = new AnimatorControllerLayer()
            {
                avatarMask = source.avatarMask,
                blendingMode = source.blendingMode,
                defaultWeight = isFirst ? 1 : source.defaultWeight,
                iKPass = source.iKPass,
                name = name,
                stateMachine = stateMachine
            };

            return layer;
        }

        private static AnimatorState CopyNewState(AnimatorState source)
        {
            AnimatorState newState = new AnimatorState()
            {
                cycleOffset = source.cycleOffset,
                cycleOffsetParameter = source.cycleOffsetParameter,
                cycleOffsetParameterActive = source.cycleOffsetParameterActive,
                hideFlags = source.hideFlags,
                iKOnFeet = source.iKOnFeet,
                mirror = source.mirror,
                mirrorParameter = source.mirrorParameter,
                mirrorParameterActive = source.mirrorParameterActive,
                motion = source.motion,
                name = source.name,
                speed = source.speed,
                speedParameter = source.speedParameter,
                speedParameterActive = source.speedParameterActive,
                tag = source.tag,
                timeParameter = source.timeParameter,
                timeParameterActive = source.timeParameterActive,
                writeDefaultValues = source.writeDefaultValues,
            };
            return newState;
        }

        private static AnimatorStateTransition CopyNewTransition(AnimatorStateMachine target, AnimatorStateTransition transitions)
        {
            AnimatorState state = FindState(target, transitions.destinationState);
            AnimatorStateMachine stateMachine = FindStateStateMachine(target, transitions.destinationStateMachine);

            AnimatorStateTransition newTransitions = new AnimatorStateTransition
            {
                canTransitionToSelf = transitions.canTransitionToSelf,
                destinationState = state,
                destinationStateMachine = stateMachine,
                duration = transitions.duration,
                exitTime = transitions.exitTime,
                hasExitTime = transitions.hasExitTime,
                hasFixedDuration = transitions.hasFixedDuration,
                hideFlags = transitions.hideFlags,
                interruptionSource = transitions.interruptionSource,
                isExit = transitions.isExit,
                mute = transitions.mute,
                name = transitions.name,
                offset = transitions.offset,
                orderedInterruption = transitions.orderedInterruption,
                solo = transitions.solo
            };

            return newTransitions;
        }

        private static StateMachineBehaviour CopyBehaviour(StateMachineBehaviour source)
        {
            System.Type type = source.GetType();
            StateMachineBehaviour newBehaviour = (StateMachineBehaviour)ScriptableObject.CreateInstance(type);

            SerializedObject serializedSource = new SerializedObject(source);
            SerializedObject serializedNew = new SerializedObject(newBehaviour);

            SerializedProperty prop = serializedSource.GetIterator();
            while (prop.Next(true))
            {
                serializedNew.CopyFromSerializedProperty(prop);
            }
            serializedNew.ApplyModifiedProperties();

            return newBehaviour;
        }
        
        public static void CopyParameters(AnimatorController target, AnimatorController source)
        {
            foreach (var t in source.parameters)
            {
                int index = FindParameterIndex(target, t.name);
                if (index != -1)
                    continue;

                AnimatorControllerParameter parameter = new AnimatorControllerParameter()
                {
                    name = t.name,
                    type = t.type,
                    defaultBool = t.defaultBool,
                    defaultFloat = t.defaultFloat,
                    defaultInt = t.defaultInt
                };

                target.AddParameter(parameter);
            }
        }

        public static void RemoveParameter(AnimatorController target, string name)
        {
            int parameter = FindParameterIndex(target, name);
            if (parameter == -1)
                return;

            target.RemoveParameter(parameter);
        }
    }
}