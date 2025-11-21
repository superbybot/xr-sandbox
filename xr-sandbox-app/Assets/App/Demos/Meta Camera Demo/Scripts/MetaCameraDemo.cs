using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Meta.XR;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace App.Demos.Meta_Camera_Demo.Scripts
{
    public class MetaCameraDemo : MonoBehaviour
    {
        [SerializeField] private OpenAIConfiguration _openAIConfiguration;
        [SerializeField] private UnityEngine.UI.RawImage _rawPicture;
        [SerializeField] private TMPro.TMP_Text _textAIMessage;
        [SerializeField] private Texture2D _testTexture;
        [SerializeField] private PassthroughCameraAccess _passthroughCameraAccess;

        private Texture2D _capturedTexture;
        private bool _isInitialized;

        private void Start()
        {
            InitializeCameraAsync();
        }

        private void InitializeCameraAsync()
        {
            if (_passthroughCameraAccess == null)
            {
                Debug.LogError("PassthroughCameraAccess component is not assigned");
                _textAIMessage.text = "PassthroughCameraAccess component is not assigned";
                return;
            }

            if (!PassthroughCameraAccess.IsSupported)
            {
                Debug.LogError("PassthroughCameraAccess is not supported on this device");
                _textAIMessage.text = "PassthroughCameraAccess is not supported on this device";
                return;
            }

            _isInitialized = true;
            Debug.Log("Pass-through camera initialized and ready");
            
            _textAIMessage.text = "Pass-through camera initialized and ready";
        }

        private void Update()
        {
            if (OVRInput.GetDown(OVRInput.Button.One))
            {
                OnAButtonPressed();
            }
        }

        private async void OnAButtonPressed()
        {
            if (!_isInitialized)
            {
                Debug.LogWarning("Camera not initialized yet");
                _textAIMessage.text = "Camera not initialized yet";
                return;
            }

            Debug.Log("A button pressed - taking picture and sending to OpenAI");

            _textAIMessage.text = "taking picture...";
            TakePicture();
            _textAIMessage.text = "finish picture...";
            // await SendImage();
        }

        public void TakePicture()
        {
            if (!_isInitialized || _passthroughCameraAccess == null)
            {
                Debug.LogWarning("Camera not initialized yet or PassthroughCameraAccess component is not assigned.");
                return;
            }

            Texture passThroughTexture = _passthroughCameraAccess.GetTexture();
            if (passThroughTexture == null)
            {
                Debug.LogError("Failed to get pass-through camera texture");
                return;
            }

            _capturedTexture = new Texture2D(passThroughTexture.width, passThroughTexture.height, TextureFormat.RGBA32, false);
            Graphics.CopyTexture(passThroughTexture, _capturedTexture);

            if (_rawPicture != null)
            {
                _rawPicture.texture = _capturedTexture;
            }

            Debug.Log($"Captured pass-through camera frame: {_capturedTexture.width}x{_capturedTexture.height}");
        }
        
        public async UniTask SendImage()
        {
            Texture2D textureToSend = GetTexture();

            if (textureToSend == null)
            {
                Debug.LogError("No texture available to send to OpenAI. Please take a picture first or assign a texture in the inspector.");
                return;
            }

            var api = new OpenAIClient(_openAIConfiguration);
            var messages = new List<Message>();
            var systemMessage = new Message(Role.System, "You are a helpful assistant");
            var contents = new List<Content>();

            contents.Add("What's in this image in 1 to 3 words.");
            contents.Add(textureToSend);

            var userMessage = new Message(Role.User, contents);
            messages.Add(systemMessage);
            messages.Add(userMessage);

            var chatRequest = new ChatRequest(messages, model: Model.GPT4o);
            var result = await api.ChatEndpoint.GetCompletionAsync(chatRequest);

            if (_textAIMessage != null)
            {
                _textAIMessage.text += "\n" + result.FirstChoice;
            }

            Debug.Log("result: " + result.FirstChoice);
        }

        private Texture2D GetTexture()
        {
            return _capturedTexture;
        }

        private void OnDestroy()
        {
            if (_capturedTexture != null)
            {
                Destroy(_capturedTexture);
            }
        }
    }
}