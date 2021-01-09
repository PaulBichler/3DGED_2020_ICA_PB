using GDLibrary.Enums;
using GDLibrary.Events;
using GDLibrary.GameComponents;
using GDLibrary.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using GDGame.MyGame.Managers;
using GDLibrary.Parameters;

namespace GDLibrary.Managers
{
    /// <summary>
    /// Stores sound effect and sound properties for a particular sound.
    /// This class demonstrates the use of the sealed keyword to prevent inheritance.
    /// </summary>
    public sealed class Cue : IDisposable
    {
        public static readonly int NO_LIMIT_SPECIFIED = -1;

        #region Fields
        private string id;
        private SoundEffect soundEffect;
        private SoundCategoryType soundCategoryType;
        private Vector3 volumePitchPan;
        private bool isLooped;

        private int maxPlayCount; //-1, 10, 1
        private int timeToLiveInMs;  //Kashmir - 45000
        private int minTimeSinceLastPlayedInMs; //1000, 60000

        #endregion Fields

        #region Properties

        public string ID
        {
            get
            {
                return id;
            }
        }

        public SoundEffect SoundEffect
        {
            get
            {
                return soundEffect;
            }
        }

        public int MaxPlayCount
        {
            get
            {
                return maxPlayCount;
            }
            set
            {
                maxPlayCount = value > 0 ? value : 1;
            }
        }

        public int TimeToLiveInMs
        {
            get
            {
                return timeToLiveInMs;
            }
            set
            {
                timeToLiveInMs = value > 0 ? value : NO_LIMIT_SPECIFIED;
            }
        }

        public int MinTimeSinceLastPlayedInMs
        {
            get
            {
                return MinTimeSinceLastPlayedInMs;
            }
            set
            {
                minTimeSinceLastPlayedInMs = value > 0 ? value : NO_LIMIT_SPECIFIED;
            }
        }

        public bool IsLooped
        {
            get
            {
                return isLooped;
            }
            set
            {
                isLooped = value;
            }
        }

        public float Volume
        {
            get
            {
                return volumePitchPan.X;
            }
            set
            {
                volumePitchPan.X = (value >= 0 && value <= 1) ? value : 1;
            }
        }

        public float Pitch
        {
            get
            {
                return volumePitchPan.Y;
            }
            set
            {
                volumePitchPan.Y = (value >= -1 && value <= 1) ? value : 0;
            }
        }

        public float Pan
        {
            get
            {
                return volumePitchPan.Z;
            }
            set
            {
                volumePitchPan.Z = (value >= -1 && value <= 1) ? value : 0;
            }
        }

        public SoundCategoryType SoundCategoryType
        {
            get
            {
                return soundCategoryType;
            }
            set
            {
                soundCategoryType = value;
            }
        }

        #endregion Properties

        public Cue(string id, SoundEffect soundEffect, SoundCategoryType soundCategoryType, Vector3 volumePitchPan, bool isLooped)
        {
            this.id = id.Trim();
            this.soundEffect = soundEffect;
            this.soundCategoryType = soundCategoryType;
            this.volumePitchPan = volumePitchPan;
            this.isLooped = isLooped;
        }

        public void Dispose()
        {
            ((IDisposable)SoundEffect).Dispose();
        }
    }

    /// <summary>
    /// Creates a dictionary of (string,Cue) pairs which we can use to play a sound by creating
    /// an instance of the SoundEffect stored within the Cue object. We can set Volume, Pitch, Pan and looping
    /// on the sound before or while it is playing.
    ///
    /// This class also demonstrates the use of the sealed keyword to prevent inheritance.
    /// </summary>
    /// <seealso cref="https://docs.monogame.net/api/Microsoft.Xna.Framework.Audio.SoundEffect.html"/>
    public sealed class SoundManager : PausableGameComponent, IEventHandler, IDisposable
    {
        private Dictionary<string, Cue> dictionary;
        private List<KeyValuePair<string, SoundEffectInstance>> listInstances2D;
        private AudioListener listener = new AudioListener();
        private AudioEmitter emitter = new AudioEmitter();
        private Transform3D listenerTransform;
        private float tempVolume;
        private bool isMuted;

        public SoundManager(Game game, StatusType statusType)
            : base(game, statusType)
        {
            dictionary = new Dictionary<string, Cue>();
            listInstances2D = new List<KeyValuePair<string, SoundEffectInstance>>();
        }

        //add events listening for Play2D etc

        public override void SubscribeToEvents()
        {
            EventDispatcher.Subscribe(EventCategoryType.Sound, HandleEvent);

            //if we always want the SoundManager to be available then comment this line out
            //base.SubscribeToEvents();
        }

        public override void HandleEvent(EventData eventData)
        {
            switch (eventData.EventActionType)
            {
                case EventActionType.OnPlay2D:
                    Play2D(eventData.Parameters[0] as string);
                    break;
                case EventActionType.OnPlay3D:
                    emitter.Position = (eventData.Parameters[1] as Transform3D).Translation;
                    listener.Position = listenerTransform.Translation;
                    Play3D(eventData.Parameters[0] as string, listener, emitter);
                    break;
                case EventActionType.OnSetListener:
                    listenerTransform = eventData.Parameters[0] as Transform3D;
                    break;
            }

            //remember to pass the eventData down so the parent class can process pause/unpause
            //if we always want the SoundManager to be available (even in menu) then comment this line out
            //base.HandleEvent(eventData);
        }

        public void Add(Cue cue)
        {
            if (!dictionary.ContainsKey(cue.ID))
            {
                dictionary.Add(cue.ID, cue);
            }
        }

        #region Sound Controls

        public void SetMasterVolume(float value)
        {
            SoundEffect.MasterVolume = MathHelper.Clamp(value, 0, 1);
        }

        public void ChangeMasterVolume(float delta)
        {
            SoundEffect.MasterVolume = MathHelper.Clamp(SoundEffect.MasterVolume + delta, 0, 1);
            isMuted = SoundEffect.MasterVolume == 0;
        }

        public void Play2D(string id) //, bool allowMultipleInstances)
        {
            id = id.Trim();

            if (dictionary.ContainsKey(id))
            {
                Cue cue = dictionary[id];
                SoundEffectInstance soundEffectInstance = dictionary[id].SoundEffect.CreateInstance();

                soundEffectInstance.Volume = cue.Volume;
                soundEffectInstance.Pitch = cue.Pitch;
                soundEffectInstance.Pan = cue.Pan;
                soundEffectInstance.IsLooped = cue.IsLooped;
                soundEffectInstance.Play();

                //store in order to support later pause and stop functionality
                KeyValuePair<string, SoundEffectInstance> pair = new KeyValuePair<string, SoundEffectInstance>(id, soundEffectInstance);
                listInstances2D.Add(pair);

                if(!cue.IsLooped)
                    TimeManager.ExecuteInSeconds(soundEffectInstance.GetHashCode().ToString(), 
                        (float)cue.SoundEffect.Duration.TotalSeconds, () => listInstances2D.Remove(pair));
            }
        }

        public void Play3D(string id, AudioListener listener, AudioEmitter emitter)
        {
            id = id.Trim();

            if (dictionary.ContainsKey(id))
            {
                Cue cue = dictionary[id];
                SoundEffectInstance soundEffectInstance = dictionary[id].SoundEffect.CreateInstance();

                soundEffectInstance.Volume = cue.Volume;
                soundEffectInstance.Pitch = cue.Pitch;
                soundEffectInstance.Pan = cue.Pan;
                soundEffectInstance.IsLooped = cue.IsLooped;
                soundEffectInstance.Apply3D(listener, emitter);
                soundEffectInstance.Play();

                //store in order to support later pause and stop functionality
                KeyValuePair<string, SoundEffectInstance> pair = new KeyValuePair<string, SoundEffectInstance>(id, soundEffectInstance);
                listInstances2D.Add(pair);

                if(!cue.IsLooped)
                    TimeManager.ExecuteInSeconds(soundEffectInstance.GetHashCode().ToString(), 
                        (float)cue.SoundEffect.Duration.TotalSeconds, () => listInstances2D.Remove(pair));
            }
        }

        public bool Pause(string id)
        {
            id = id.Trim();
            foreach (KeyValuePair<string, SoundEffectInstance> pair in listInstances2D)
            {
                if (pair.Key.Equals(id))
                {
                    SoundEffectInstance instance = pair.Value;
                    if (instance.State == SoundState.Playing)
                    {
                        instance.Pause();
                        return true;
                    }
                }
            }

            return false;
        }

        public bool Resume(string id)
        {
            id = id.Trim();
            foreach (KeyValuePair<string, SoundEffectInstance> pair in listInstances2D)
            {
                if (pair.Key.Equals(id))
                {
                    SoundEffectInstance instance = pair.Value;
                    if (instance.State == SoundState.Paused)
                    {
                        instance.Resume();
                        return true;
                    }
                }
            }
            return false;
        }

        public bool Stop(string id)
        {
            id = id.Trim();
            bool bFound = false;

            //foreach (KeyValuePair<string, SoundEffectInstance> pair in this.listInstances2D)
            for (int i = 0; i < listInstances2D.Count; i++)
            {
                KeyValuePair<string, SoundEffectInstance> pair = listInstances2D[i];
                if (pair.Key.Equals(id))
                {
                    SoundEffectInstance instance = pair.Value;
                    if (instance.State == SoundState.Playing)
                    {
                        instance.Stop();
                        listInstances2D.Remove(pair);
                        bFound = true;
                        break;
                    }
                }
            }
            return bFound;
        }

        public bool SetVolume(string id, float volume)
        {
            id = id.Trim();
            foreach (KeyValuePair<string, SoundEffectInstance> pair in listInstances2D)
            {
                if (pair.Key.Equals(id))
                {
                    SoundEffectInstance instance = pair.Value;

                    //playable and pausable sounds can be changed in volume
                    if (instance.State == SoundState.Playing || instance.State == SoundState.Paused)
                    {
                        instance.Volume = volume;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool ChangeVolume(string id, float delta)
        {
            id = id.Trim();
            foreach (KeyValuePair<string, SoundEffectInstance> pair in listInstances2D)
            {
                if (pair.Key.Equals(id))
                {
                    SoundEffectInstance instance = pair.Value;

                    //playable and pausable sounds can be changed in volume
                    if (instance.State == SoundState.Playing || instance.State == SoundState.Paused)
                    {
                        float newVolume = instance.Volume + delta;
                        if (newVolume >= 0 && newVolume <= 1)
                        {
                            instance.Volume = newVolume;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void ToggleMute()
        {
            if (isMuted)
            {
                SetMasterVolume(tempVolume);
                isMuted = false;
            }
            else
            {
                tempVolume = SoundEffect.MasterVolume;
                SetMasterVolume(0);
                isMuted = true;
            }
        }

        #endregion Sound Controls

        //3D Sound Controls

        //Event Handling - EventCategoryType.Sound2D, EventActionType.Play2D, ["boom",1, -1, 1, true]

        public new void Dispose()
        {
            ///compare this method to calling Dispose on dictionary contents to ContentDictionary::Dispose()
            ///see https://robertgreiner.com/iterating-through-a-dictionary-in-csharp/
            foreach (KeyValuePair<string, Cue> pair in dictionary)
            {
                ((IDisposable)pair.Value).Dispose();
            }

            dictionary.Clear();
        }
    }
}