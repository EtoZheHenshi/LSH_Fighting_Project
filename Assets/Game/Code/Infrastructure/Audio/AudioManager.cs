using System.Collections;
using UnityEngine;

namespace Code.Infrastructure.Audio
{
    public sealed class AudioManager : Singleton<AudioManager>
    {
        [SerializeField] private AudioSourcePool pool;
        
        private AudioSource _playerMusicSource;
        private float _playerTrackLengthMs;
        
        public AudioSource PlayerMusicSource => _playerMusicSource;
        public float PlayerTrackLengthMs => _playerTrackLengthMs;
        public bool PlayerMusicSet { get; private set; }

        public void PlaySound(SoundData soundData, float duration = -1f)
        {
            AudioSource source = pool.GetAudioSource();
            
            source.clip = soundData.GetClip();
            source.pitch = soundData.GetPitch();
            source.volume = soundData.volume;

            source.Play();

            if (duration < 0f)
            {
                duration = source.clip.length;
            }
            
            StartCoroutine(ReturnToPool(source, duration));
        }

        public void SetPlayerMusic(SoundData soundData)
        {
            _playerMusicSource = pool.GetAudioSource();
            
            _playerMusicSource.clip = soundData.GetClip();
            _playerMusicSource.pitch = soundData.GetPitch();
            _playerMusicSource.volume = soundData.volume;
            
            _playerTrackLengthMs = _playerMusicSource.clip.length * 1000f;
            
            PlayerMusicSet = true;
        }

        public void PlayPlayerMusic()
        {
            _playerMusicSource.Play();
        }

        public void PausePlayerMusic()
        {
            _playerMusicSource.Pause();
        }

        private IEnumerator ReturnToPool(AudioSource audioSource, float delay)
        {
            yield return new WaitForSeconds(delay);
            audioSource.Stop();
            pool.ReturnAudioSource(audioSource);
        }
    }
}