using UnityEngine;

namespace Knifest.DebugTools
{
    public class FpsCounter : MonoBehaviour
    {
        [SerializeField] private TMPro.TMP_Text _textUI;
        [SerializeField] private int _bufferSize;
        [SerializeField] private int _maxFps;
        [SerializeField] private int _minDelta;

        private int[] _buffer;
        private int _index;
        private int _bufferCount;
        private int _lastFps;
        private string[] _int2str;

        private void Awake()
        {
            _buffer = new int[_bufferSize];
            _index = 0;
            _bufferCount = 0;
            _lastFps = 0;

            _int2str = new string[_maxFps + 1];
            for (int i = 0; i <= _maxFps; i++)
            {
                _int2str[i] = i.ToString();
            }
        }

        private void Update()
        {
            _buffer[_index] = (int)(1f / Time.unscaledDeltaTime);
            _index = ++_index % _bufferSize;
            if (++_bufferCount > _bufferSize)
                _bufferCount = _bufferSize;
            ShowFps();
        }

        private void ShowFps()
        {
            int sum = 0;
            for (int i = 0; i < _bufferCount; i++)
                sum += _buffer[i];
            int fps = (int)((float)sum / _bufferCount);

            if (fps - _minDelta > _lastFps || fps + _minDelta < _lastFps)
            {
                fps = fps > _maxFps ? _maxFps : fps;
                _textUI.text = _int2str[fps];
                _lastFps = fps;
            }
        }
    }
}