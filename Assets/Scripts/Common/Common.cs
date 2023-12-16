using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MasyoLab.GameFramework.Utils;
using MasyoLab.GameFramework.Manager;
using MasyoLab.GameFramework.Input;

namespace MasyoLab.Game
{
    public class Common : SingletonMonoBehaviour<Common>
    {
        public ResourceManager ResourceManager => ResourceManager.Inst;
        public ParamManager ParamManager => ParamManager.Inst;
        public SceneManager SceneManager => SceneManager.Inst;
        public Gamepad Gamepad => Gamepad.Inst;
        public Keyboard Keyboard => Keyboard.Inst;

        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            new GameObject("Common", typeof(Common));
        }

        protected override void Awake()
        {
            base.Awake();
            gameObject.AddComponent<ResourceManager>();
            gameObject.AddComponent<ParamManager>();
            gameObject.AddComponent<SceneManager>();
            gameObject.AddComponent<Gamepad>();
            gameObject.AddComponent<Keyboard>();
        }
    }
}
