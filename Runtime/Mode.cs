using System;
using System.Collections.Generic;
using UnityEngine;


namespace ModeMachine
{
    public abstract class Mode : MonoBehaviour
    {
        //channels
        internal List<ChannelID> channels = new List<ChannelID>();
        protected void SetChannel(ChannelID channel, bool value)
        {
            //presence of ChannelID on channels list indicates a true value
            if(value == false)
            {
                if (channels.Contains(channel))
                    channels.Remove(channel);
            }
            else
            {
                if (!channels.Contains(channel))
                    channels.Add(channel);
            }
        }
        public bool GetChannel(ChannelID channel)
        {
            return channels.Contains(channel);
        }

        /// <summary>
        /// Returns true if the mode has all channels in channelFilter
        /// </summary>
        public bool CheckChannelFilter(params ChannelID[] channelFilter)
        {
            for (int n = 0; n < channelFilter.Length; n++)
            {
                if (!GetChannel(channelFilter[n]))
                {
                    return false;
                }
            }
            return true;
        }

        public abstract int GetDepth(params ChannelID[] channelFilter);
        public abstract int GetDepthFull(params ChannelID[] channelFilter);
        internal abstract int GetChildCountRecursive(params ChannelID[] channelFilter);

        internal Mode(){}//constructor is internal to force user to inherit from generic Mode<>
        internal IModeStack ParentStack;//user should not be able to change ParentStack manually

        //events
        internal Action WasPushed,
            WasRemoved,
            StackWasChanged;

        //state
        private bool initialized = false;
        private bool applicationQuitting = false;
        
        //subscribe to delegates
        internal void InitializeIfNeeded()
        {
            if (initialized)
                return;
            initialized = true;
            Application.quitting += DetectQuit;
            WasPushed += OnPush;
            WasRemoved += OnRemove;
            StackWasChanged += OnStackChanged;
            ModeDelegateSubscriptionManager.Instance.RegisterMode(this);
        }

        internal void CleanupDelegates()
        {
            Application.quitting -= DetectQuit;
            WasPushed -= OnPush;
            WasRemoved -= OnRemove;
            StackWasChanged -= OnStackChanged;
        }

        //need to know if application is quitting so we don't call stack changed events from ondestroy
        void DetectQuit()
        {
            applicationQuitting = true;
            Application.quitting -= DetectQuit;
        }

        protected virtual void OnDestroy()
        {
            CleanupDelegates();
            if (ParentStack != null && !applicationQuitting)
                ParentStack.RemoveMode(this);
        }

        protected virtual void OnPush(){}
        protected virtual void OnRemove() { }
        protected virtual void OnStackChanged() { }
        
        //bookkeeping functionality
        internal abstract bool ValidateParentStackType(IModeStack newStack);
    }
}