import "../style/css/animations.css";
import { useEffect } from 'react';

function useOnClickOutside(ref, handler) {
  useEffect(
    () => {
      const listener = event => {
        if (!ref.current || ref.current.contains(event.target)) {
          return;
        }
        handler(event);
      };

      document.addEventListener('mousedown', listener);
      document.addEventListener('touchstart', listener);

      return () => {
        document.removeEventListener('mousedown', listener);
        document.removeEventListener('touchstart', listener);
      };
    },
    [ref, handler]
  );
}

function useRippleEffect(ref) {

  const onClick = () => {
    let positionProperty = ref.current.style.position;
    let overflowProperty = ref.current.style.overflow;

    ref.current.style.position = "relative";
    ref.current.style.overflow = "hidden";

    document.documentElement.style.setProperty("--ripple-size", `${ref.current.getBoundingClientRect().width * 1.5}px`);

    const rippleDiv = document.createElement("div");
    rippleDiv.className = "ripple";

    ref.current.appendChild(rippleDiv);
    setTimeout(() => {
      ref.current.removeChild(rippleDiv);
      ref.current.style.position = positionProperty;
      ref.current.style.overflow = overflowProperty;
    }, 300);
  }

  useEffect(() => {
    if (ref.current) {
      ref.current.addEventListener('click', onClick);
    }

    return () => {
      ref.current?.removeEventListener('click', onClick);
    }
  }, [ref])
}

function useFadeTransition(ref, visible) {

  useEffect(() => {
    if (visible) {
      ref.current.className = ref.current.className.replace("fade-out", "");
      ref.current.className += " fade-in";
    }
    else {
      ref.current.className = ref.current.className.replace("fade-in", "");
      ref.current.className += " fade-out";
    }

  }, [ref, visible])
}

export { useOnClickOutside, useRippleEffect, useFadeTransition };