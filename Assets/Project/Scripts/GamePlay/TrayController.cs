using System.Collections.Generic;
using UnityEngine;
using ZenMatch.Data;
using ZenMatch.UI;

namespace ZenMatch.Gameplay
{
    [DisallowMultipleComponent]
    public sealed class TrayController : MonoBehaviour
    {
        [SerializeField] private int capacity = 7;
        [SerializeField] private TrayView trayView;

        private TrayState _state;

        public TrayState State => _state;
        public TrayView View => trayView;
        public bool IsFull => _state != null && _state.IsFull;

        public void Initialize()
        {
            InitializeWithActiveCapacity(capacity);
        }

        public void InitializeWithActiveCapacity(int activeCapacity)
        {
            _state = new TrayState(capacity);
            _state.SetActiveCapacity(activeCapacity);
            RefreshView();
        }

        public void ApplyActiveCapacity(int activeCapacity)
        {
            if (_state == null)
                _state = new TrayState(capacity);

            _state.SetActiveCapacity(activeCapacity);
            RefreshView();
        }

        public bool UnlockOneLockedSlot()
        {
            if (_state == null)
                Initialize();

            bool unlocked = _state.UnlockOneLockedSlot();
            if (unlocked)
                RefreshView();

            return unlocked;
        }



        public bool UnlockOneLockedSlot(out int unlockedSlotIndex)
        {
            unlockedSlotIndex = -1;

            if (_state == null)
                Initialize();

            int beforeCapacity = _state.CurrentCapacity;

            bool unlocked = _state.UnlockOneLockedSlot();

            if (unlocked)
            {
                unlockedSlotIndex = beforeCapacity;
                RefreshView();
            }

            return unlocked;
        }

        public void ResetTray()
        {
            if (_state == null)
                _state = new TrayState(capacity);
            else
                _state.ClearAll();

            RefreshView();
        }

        public void RestoreSlots(IReadOnlyList<TileTypeSO> slots)
        {
            if (_state == null)
                _state = new TrayState(capacity);

            _state.SetSlots(slots);
            RefreshView();
        }

        public bool TryAddTile(TileTypeSO tileType, out bool clearedAny)
        {
            clearedAny = false;

            if (_state == null)
                Initialize();

            if (!_state.CanAdd())
            {
                Debug.Log("[TrayController] Tray dolu, tile eklenemedi.");
                return false;
            }

            _state.Add(tileType);

            if (_state.FindTripleIndices(out List<int> matchedSlotIndices))
            {
                clearedAny = true;

                List<TileTypeSO> beforeSlots = new List<TileTypeSO>(_state.Slots);

                _state.RemoveIndices(matchedSlotIndices);

                List<TileTypeSO> afterSlots = new List<TileTypeSO>(_state.Slots);

                if (trayView != null)
                {
                    trayView.PlayMatchResolveSequence(
                        beforeSlots,
                        matchedSlotIndices,
                        afterSlots,
                        _state.CurrentCapacity,
                        _state.MaxVisualCapacity,
                        _state.LockedSlots);
                }
                else
                {
                    RefreshView();
                }

                return true;
            }

            RefreshView();
            return true;
        }

        public void RefreshView()
        {
            if (trayView != null)
                trayView.Rebuild(_state);
        }
    }
}