// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/03/27 19:02
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

#if false // MODULE_MARKER
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using TMPro;

namespace DG.Tweening
{
    /// <summary>
    /// Methods that extend TMP_Text objects and allow to directly create and control tweens from their instances.
    /// </summary>
    public static class ShortcutExtensionsTMPText
    {
        #region Colors

        /// <summary>Tweens a TextMeshPro's color to the given value.
        /// Also stores the TextMeshPro as the tween's inTarget so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Color, Color, ColorOptions> DOColor(this TMP_Text inTarget, Color endValue, float duration)
        {
            TweenerCore<Color, Color, ColorOptions> t = DOTween.To(() => inTarget.color, x => inTarget.color = x, endValue, duration);
            t.SetTarget(inTarget);
            return t;
        }

        /// <summary>Tweens a TextMeshPro's faceColor to the given value.
        /// Also stores the TextMeshPro as the tween's inTarget so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Color, Color, ColorOptions> DOFaceColor(this TMP_Text inTarget, Color32 endValue, float duration)
        {
            TweenerCore<Color, Color, ColorOptions> t = DOTween.To(() => inTarget.faceColor, x => inTarget.faceColor = x, endValue, duration);
            t.SetTarget(inTarget);
            return t;
        }

        /// <summary>Tweens a TextMeshPro's outlineColor to the given value.
        /// Also stores the TextMeshPro as the tween's inTarget so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Color, Color, ColorOptions> DOOutlineColor(this TMP_Text inTarget, Color32 endValue, float duration)
        {
            TweenerCore<Color, Color, ColorOptions> t = DOTween.To(() => inTarget.outlineColor, x => inTarget.outlineColor = x, endValue, duration);
            t.SetTarget(inTarget);
            return t;
        }

        /// <summary>Tweens a TextMeshPro's glow color to the given value.
        /// Also stores the TextMeshPro as the tween's inTarget so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        /// <param name="useSharedMaterial">If TRUE will use the fontSharedMaterial instead than the fontMaterial</param>
        public static TweenerCore<Color, Color, ColorOptions> DOGlowColor(this TMP_Text inTarget, Color endValue, float duration, bool useSharedMaterial = false)
        {
            TweenerCore<Color, Color, ColorOptions> t = useSharedMaterial
                ? inTarget.fontSharedMaterial.DOColor(endValue, "_GlowColor", duration)
                : inTarget.fontMaterial.DOColor(endValue, "_GlowColor", duration);
            t.SetTarget(inTarget);
            return t;
        }

        /// <summary>Tweens a TextMeshPro's alpha color to the given value.
        /// Also stores the TextMeshPro as the tween's inTarget so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Color, Color, ColorOptions> DOFade(this TMP_Text inTarget, float endValue, float duration)
        {
            TweenerCore<Color, Color, ColorOptions> t = DOTween.ToAlpha(() => inTarget.color, x => inTarget.color = x, endValue, duration);
            t.SetTarget(inTarget);
            return t;
        }

        /// <summary>Tweens a TextMeshPro faceColor's alpha to the given value.
        /// Also stores the TextMeshPro as the tween's inTarget so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Color, Color, ColorOptions> DOFaceFade(this TMP_Text inTarget, float endValue, float duration)
        {
            TweenerCore<Color, Color, ColorOptions> t = DOTween.ToAlpha(() => inTarget.faceColor, x => inTarget.faceColor = x, endValue, duration);
            t.SetTarget(inTarget);
            return t;
        }

        #endregion

        #region Other

        /// <summary>Tweens a TextMeshPro's scale to the given value (using correct uniform scale as TMP requires).
        /// Also stores the TextMeshPro as the tween's inTarget so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<Vector3, Vector3, VectorOptions> DOScale(this TMP_Text inTarget, float endValue, float duration)
        {
            Transform trans = inTarget.transform;
            Vector3 endValueV3 = new Vector3(endValue, endValue, endValue);
            TweenerCore<Vector3, Vector3, VectorOptions> t = DOTween.To(() => trans.localScale, x => trans.localScale = x, endValueV3, duration);
            t.SetTarget(inTarget);
            return t;
        }

        /// <summary>Tweens a TextMeshPro's fontSize to the given value.
        /// Also stores the TextMeshPro as the tween's inTarget so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<float, float, FloatOptions> DOFontSize(this TMP_Text inTarget, float endValue, float duration)
        {
            TweenerCore<float, float, FloatOptions> t = DOTween.To(() => inTarget.fontSize, x => inTarget.fontSize = x, endValue, duration);
            t.SetTarget(inTarget);
            return t;
        }

        /// <summary>Tweens a TextMeshPro's maxVisibleCharacters to the given value.
        /// Also stores the TextMeshPro as the tween's inTarget so it can be used for filtered operations</summary>
        /// <param name="endValue">The end value to reach</param><param name="duration">The duration of the tween</param>
        public static TweenerCore<int, int, NoOptions> DOMaxVisibleCharacters(this TMP_Text inTarget, int endValue, float duration)
        {
            TweenerCore<int, int, NoOptions> t = DOTween.To(() => inTarget.maxVisibleCharacters, x => inTarget.maxVisibleCharacters = x, endValue, duration);
            t.SetTarget(inTarget);
            return t;
        }

        /// <summary>Tweens a TextMeshPro's text to the given value.
        /// Also stores the TextMeshPro as the tween's inTarget so it can be used for filtered operations</summary>
        /// <param name="endValue">The end string to tween to</param><param name="duration">The duration of the tween</param>
        /// <param name="richTextEnabled">If TRUE (default), rich text will be interpreted correctly while animated,
        /// otherwise all tags will be considered as normal text</param>
        /// <param name="scrambleMode">The type of scramble mode to use, if any</param>
        /// <param name="scrambleChars">A string containing the characters to use for scrambling.
        /// Use as many characters as possible (minimum 10) because DOTween uses a fast scramble mode which gives better results with more characters.
        /// Leave it to NULL (default) to use default ones</param>
        public static TweenerCore<string, string, StringOptions> DOText(this TMP_Text inTarget, string endValue, float duration, bool richTextEnabled = true, ScrambleMode scrambleMode = ScrambleMode.None, string scrambleChars = null)
        {
            TweenerCore<string, string, StringOptions> t = DOTween.To(() => inTarget.text, x => inTarget.text = x, endValue, duration);
            t.SetOptions(richTextEnabled, scrambleMode, scrambleChars)
                .SetTarget(inTarget);
            return t;
        }

        #endregion
    }
}
#endif
