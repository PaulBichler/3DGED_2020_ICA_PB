using GDLibrary.Containers;
using GDLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace GDLibrary.Events
{
    /// <summary>
    /// Creates a dictionary where each key-value pair is a list of events and the time delay in MS after which the event is fired.
    ///
    /// Note:
    ///      - Events do NOT need to be added in CHRONOLOGICAL order
    ///      - Supports TWO OR MORE events occuring at the same time
    ///
    /// Usage:
    ///     EventScheduler scheduler = new EventScheduler("camera events");
    ///     scheduler.Add(new EventData(EventCategoryType.Camera, EventActionType.OnStop), 12000);
    ///     scheduler.Add(new EventData(EventCategoryType.Camera, EventActionType.OnPause), 10000);
    ///     scheduler.Add(new EventData(EventCategoryType.Camera, EventActionType.OnPlay), 7000);
    ///     scheduler.Add(new EventData(EventCategoryType.Camera, EventActionType.OnRestart), 7000);
    ///     scheduler.Add(new EventData(EventCategoryType.Camera, EventActionType.OnResume), 7000);
    ///     scheduler.Add(new EventData(EventCategoryType.Scheduler,
    ///     EventActionType.OnResetRestart, ["camera events"]), 60000);
    ///
    ///     scheduler.Add(new EventData(EventCategoryType.Scheduler,
    ///     EventActionType.OnAdd, [eventData, 10000]), 7000);
    ///
    ///
    ///     scheduler.Start();
    ///
    /// In addition we can start, stop, and reset the scheduler using events e.g.
    ///
    ///     EventDispatcher.Publish(new EventData(EventCategoryType.Scheduler, EventActionType.OnStart));
    ///     EventDispatcher.Publish(new EventData(EventCategoryType.Scheduler, EventActionType.OnStop));
    ///     EventDispatcher.Publish(new EventData(EventCategoryType.Scheduler, EventActionType.OnReset));
    ///
    /// </summary>
    public class EventScheduler
    {
        #region Fields
        private string id;
        private Timer timer;
        private List<Pair<int, List<EventData>>> scheduledEventList;
        private List<Pair<int, List<EventData>>> copyScheduledEventList; //copy of original list
        private StateType bStatus;
        #endregion Fields

        #region Properties
        public string ID { get => id; set => id = value.Trim(); }
        public StateType Status { get => bStatus; private set => bStatus = value; }
        #endregion Properties

        #region Constructors & Core

        /// <summary>
        /// Constructs a scheduler
        /// </summary>
        /// <param name="id">String id which can be used to specify the collecion of events (e.g. sound events)</param>
        public EventScheduler(string id)
        {
            ID = id;
            scheduledEventList = new List<Pair<int, List<EventData>>>();
            SubscribeToEvents();
        }

        /// <summary>
        /// Adds support for
        /// - subsribing to events to add EventData objects to the scheduler
        /// - starting the scheduler
        /// - stopping the scheduler
        /// </summary>
        protected virtual void SubscribeToEvents()
        {
            EventDispatcher.Subscribe(EventCategoryType.Scheduler, HandleEvent);
        }

        /// <summary>
        /// Handles events sent to the event scheduler with the targetID listed in Parameters[0]
        /// </summary>
        /// <param name="eventData"></param>
        protected virtual void HandleEvent(EventData eventData)
        {
            string targetID = eventData.Parameters[0] as string;
            if (targetID != null)
            {
                targetID.Trim();

                if (targetID.Equals(this.ID))
                {
                    switch (eventData.EventActionType)
                    {
                        case EventActionType.OnAdd:
                            Add(eventData.Parameters[1] as EventData,
                                        (int)eventData.Parameters[2]);
                            break;

                        case EventActionType.OnStart:
                            Start();
                            break;

                        case EventActionType.OnStop:
                            Stop();
                            break;

                        case EventActionType.OnReset:
                            Reset();
                            break;

                        case EventActionType.OnResetStart:
                            Reset();
                            Start();
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Call this method to add an event to the scheduler
        /// </summary>
        /// <param name="eventData">EventData</param>
        /// <param name="delayInMs">Integer delay in ms</param>
        /// <returns></returns>
        public virtual bool Add(EventData eventData, int delayInMs)
        {
            bool bAdded = false;
            int findIndex = scheduledEventList.FindIndex(pair => pair.Key == delayInMs);

            if (findIndex == -1)
            {
                List<EventData> newList = new List<EventData>();
                newList.Add(eventData);
                scheduledEventList.Add(new Pair<int, List<EventData>>(delayInMs, newList));
                bAdded = true;
            }
            else
            {
                if (!scheduledEventList.ElementAt(findIndex).Value.Contains(eventData))
                {
                    scheduledEventList.ElementAt(findIndex).Value.Add(eventData);
                    bAdded = true;
                }
            }

            return bAdded;
        }

        /// <summary>
        /// Call this method to Start the scheduler
        /// </summary>
        public virtual void Start()
        {
            System.Diagnostics.Debug.WriteLine("Starting...");

            if (scheduledEventList.Count > 0)
            {
                //sort the list so events are in chronological order
                scheduledEventList.Sort((a, b) => a.Key - b.Key);

                //copy the final list of events in case we want to call Reset and restart the scheduler
                if (copyScheduledEventList != null)
                    copyScheduledEventList.Clear();

                copyScheduledEventList = GetListCopy(scheduledEventList);

                //start first timer and update delay until all events after the first
                StartTimerAndUpdate();

                //set scheduler status
                Status = StateType.Running;
            }
        }

        /// <summary>
        /// Call this method to stop all outstanding timers and clear the list
        /// </summary>
        public virtual void Stop()
        {
            StopTimer();
            scheduledEventList.ForEach(pair => pair.Value.Clear());
            scheduledEventList.Clear();
        }

        /// <summary>
        /// Allows us to restart the scheduler from T=0 again
        /// </summary>
        public virtual void Reset()
        {
            if (timer != null)
                throw new Exception("Do not reset scheduler while it is still running! Call stop first.");

            scheduledEventList.ForEach(pair => pair.Value.Clear());
            scheduledEventList.Clear();
            scheduledEventList = GetListCopy(copyScheduledEventList);
        }

        /// <summary>
        /// Updates all events AFTER first event by substracting the time of the first event
        /// </summary>
        protected virtual void StartTimerAndUpdate()
        {
            //calculate the delay for the next event to be published
            int delayInMs = scheduledEventList[0].Key;

            //update all events AFTER the first event to substract the time spent to wait until the first
            //subtract the time for the next event to be published from all other events
            for (int i = 1; i < scheduledEventList.Count; i++)
            {
                scheduledEventList[i].Key -= delayInMs;
            }

            //start time timer with the delay until the next event
            StartTimer(delayInMs);
        }

        /// <summary>
        /// Called when a timed interval between events occurs and starts the next timer in the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void HandleTimerElapsed(object sender, ElapsedEventArgs e)
        {
            //publish the event(s) listed in position = 0
            foreach (EventData eventData in scheduledEventList[0].Value)
                EventDispatcher.Publish(eventData);

            //clear the list at position = 0
            scheduledEventList[0].Value.Clear();

            //remove the KeyValuePair at position = 0
            scheduledEventList.RemoveAt(0);

            //if events then start the timer for the next list of events in scheduledEventList
            if (scheduledEventList.Count != 0)
            {
                //start first timer and update delay until all events after the first
                StartTimerAndUpdate();
            }
            //if no more events then release the resource from the last run timer
            else
            {
                StopTimer();
                Status = StateType.Stopped;
            }
        }

        protected virtual void StartTimer(int delayInMs)
        {
            StopTimer();
            timer = new Timer(delayInMs);
            timer.AutoReset = false;
            timer.Elapsed += HandleTimerElapsed;
            timer.Start();
        }

        protected virtual void StopTimer()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
                timer = null;
            }
        }

        protected virtual List<Pair<int, List<EventData>>> GetListCopy(List<Pair<int, List<EventData>>> list)
        {
            List<Pair<int, List<EventData>>> copy = new List<Pair<int, List<EventData>>>();

            foreach (Pair<int, List<EventData>> pair in list)
            {
                List<EventData> newList = new List<EventData>();
                foreach (EventData eventData in pair.Value)
                    newList.Add(eventData.Clone() as EventData);

                copy.Add(new Pair<int, List<EventData>>(pair.Key, newList));
            }

            return copy;
        }

        #endregion Constructors & Core
    }

    public enum StateType : sbyte
    {
        Running,
        Stopped
    }
}