using System;
using UnityEngine;

namespace Chameleon.Monobehaviors
{
    public class SubEnterHandTarget : HandTarget, IHandTarget
    {
        //Blatanly stolen from the Seal sub. Go check them out on their discord server!
        //https://discord.com/invite/R2xCt9zXff

        public SubRoot subRoot;
        public Transform targetPosition;
        public FMODAsset sound;
        public Action OnEnter;

        public void OnHandClick(GUIHand hand)
        {
            if(sound != null)
            {
                Utils.PlayFMODAsset(sound, targetPosition.position);
            }

            Player.main.SetPosition(targetPosition.position, targetPosition.rotation);
            Player.main.SetCurrentSub(subRoot, true);

            OnEnter.Invoke();
        }

        public void OnHandHover(GUIHand hand)
        {
            string enterText = Language.main.Get("ChameleonEnter");
            string exitText = Language.main.Get("ChameleonExit");

            string text = (subRoot ? enterText : exitText);
            HandReticle.main.SetText(HandReticle.TextType.Hand, text, true, GameInput.Button.LeftHand);
            HandReticle.main.SetText(HandReticle.TextType.HandSubscript, string.Empty, false, GameInput.Button.None);
            HandReticle.main.SetIcon(HandReticle.IconType.Hand, 1f);
        }
    }
}
