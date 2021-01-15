using System.Collections.Generic;
using UnityEngine;

namespace ModeMachine
{
    public abstract class Mode<T> : Mode where T : class, IModeStack
    {
        //this gives a type-correct reference to the stack the mode is on
        new public T ParentStack { get { return base.ParentStack as T; } }

        public sealed override int GetDepth(params ChannelID[] channelFilter)
        {
            if (ParentStack == null)
                return -1;
            if (channelFilter.Length == 0)
                return ParentStack.ModeStack.Modes.IndexOf(this);

            //create a list of all modes that have the correct channels (plus this mode)
            List<Mode> modes = new List<Mode>(ParentStack.ModeStack.Modes);
            for (int i = modes.Count; i > 0;)
            {
                i--;
                if (modes[i] == this)
                    continue;

                if (!modes[i].CheckChannelFilter(channelFilter))
                {
                    modes.RemoveAt(i);
                }
            }
            return modes.IndexOf(this);
        }

        internal override int GetChildCountRecursive(params ChannelID[] channelFilter)
        {
            IModeStack stack = this as IModeStack;
            if(stack == null)
                return 0;

            int result = 0;
            for(int i = 0; i < stack.ModeStack.Modes.Count; i++)
            {
                if(stack.ModeStack.Modes[i].CheckChannelFilter(channelFilter))
                {
                    result++;
                }
                result += stack.ModeStack.Modes[i].GetChildCountRecursive(channelFilter);
            }
            return result;
        }

        public sealed override int GetDepthFull(params ChannelID[] channelFilter)
        {
            if (ParentStack == null)
                return -1;

            int index = GetDepth();
            int totalDepth = 0;
            for(int i = index; i > 0;)
            {
                i--;
                if(ParentStack.ModeStack.Modes[i].CheckChannelFilter(channelFilter))
                {
                    totalDepth++;
                }
                totalDepth += ParentStack.ModeStack.Modes[i].GetChildCountRecursive(channelFilter);
            }

            //hmm not fully sure if this will work
            Mode parentMode = ParentStack as Mode;
            while (parentMode != null)
            {
                if (parentMode.CheckChannelFilter() && parentMode.ParentStack != null)
                    totalDepth++;
                index = parentMode.GetDepth();
                if (index <= 0)
                    break;
                index--;

                totalDepth += parentMode.GetChildCountRecursive(channelFilter);
                parentMode = parentMode.ParentStack as Mode;
            }
            return totalDepth;
        }

        internal override bool ValidateParentStackType(IModeStack newStack)
        {
            if (newStack as T == null)
            {
                Debug.LogErrorFormat("Trying to push {0} mode to the wrong type of stack. Was {1}, should be {2}", 
                    gameObject == null ? "[null game object]" : gameObject.name, 
                    newStack == null ? "[null stack]" : newStack.GetType().ToString(),
                    typeof(T).ToString()); ;
                return false;
            }
            return true;
        }
    }
}
