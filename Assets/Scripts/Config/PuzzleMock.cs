using System;
using UnityEngine;
using ViewModel;

namespace Config
{
    [Serializable]
    public class PuzzleMock
    {
        public string Id;
        public Sprite Image;
        public PuzzleOpenType OpenType;
    }
}