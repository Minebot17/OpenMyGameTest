using System;
using System.Collections.Generic;
using R3;
using UnityEngine;
using VContainer;
using View.Common;
using View.Common.Transitions;
using ViewModel;

namespace View
{
    public class PuzzleSelectionPanelView : PanelView<PuzzleSelectionPanelViewModel>
    {
        [SerializeField] private PuzzleSelectionElementView _puzzleElementViewPrefab;
        [SerializeField] private RectTransform _puzzleElementViewContainer;
        
        private IObjectResolver _objectResolver;
        
        private readonly Dictionary<string, PuzzleSelectionElementView> _puzzleElements = new();
        
        protected override Type TransitionType => typeof(AlphaPanelTransition);

        [Inject]
        public void Construct(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }
        
        protected override void OnBindInternal(CompositeDisposable disposables)
        {
            ViewModel.PuzzleSelectionElements
                .BindViews(_puzzleElements, _objectResolver, _puzzleElementViewPrefab, _puzzleElementViewContainer)
                .AddTo(disposables);
        }
    }
}