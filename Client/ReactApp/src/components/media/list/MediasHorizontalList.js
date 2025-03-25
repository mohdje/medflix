import "../../../style/css/medias-horizontal-list.css";
import "../../../style/css/button.css";

import MediaLitePresentation from "../presentation/MediaLitePresentation.js";

import SemiArrowLeft from '../../../assets/left_semi_arrow.svg'
import SemiArrowRight from '../../../assets/right_semi_arrow.svg'

import { useFadeTransition } from "../../../helpers/customHooks.js";
import { useEffect, useRef, useState } from 'react';

function MediasHorizontalList({ title, centerTitle, medias, onMediaClick }) {
  const listRef = useRef(null);
  const rightArrowRef = useRef(null);
  const leftArrowRef = useRef(null);

  const [hideBackArrow, setHideBackArrow] = useState(true);
  const [hideForwardArrow, setHideForwardArrow] = useState(false);

  useFadeTransition(leftArrowRef, !hideBackArrow);
  useFadeTransition(rightArrowRef, !hideForwardArrow);

  useEffect(() => {
    listRef.current.onscroll = () => {
      showHideNavArrows();
    };
  }, []);

  useEffect(() => {
    if (!medias || medias.length === 0) {
      setHideForwardArrow(true);
      setHideBackArrow(true);
    }
    else showHideNavArrows();
  }, [medias]);

  useEffect(() => { showHideNavArrows(); }, [hideBackArrow, hideForwardArrow]);

  const isElementVisible = (elem, boundings) => {
    const elemBoundings = elem.getBoundingClientRect();
    return Math.floor(elemBoundings.right) <= Math.floor(boundings.right)
      && Math.floor(elemBoundings.left) >= Math.floor(boundings.left);
  }

  const getVisibleRange = () => {
    var indexes = []
    var listBoundings = listRef.current.getBoundingClientRect();
    for (let i = 0; i < listRef.current.children.length; i++) {
      if (isElementVisible(listRef.current.children[i], listBoundings))
        indexes.push(i);
    }

    return {
      'maxIndex': Math.max(...indexes),
      'minIndex': Math.min(...indexes),
      'count': indexes.length
    }
  }

  const navToRight = () => {
    if (listRef.current.children && listRef.current.children.length > 0) {
      var visibleRange = getVisibleRange();
      var goToIndex = visibleRange.maxIndex + visibleRange.count;
      var index = goToIndex > listRef.current.children.length - 1 ? listRef.current.children.length - 1 : goToIndex;

      listRef.current.children[index].scrollIntoView({
        behavior: 'smooth',
        block: "nearest",
        inline: "nearest"
      });
    }
  }

  const navToLeft = () => {
    if (listRef.current.children && listRef.current.children.length > 0) {
      var visibleRange = getVisibleRange();
      var goToIndex = visibleRange.minIndex - visibleRange.count;
      var index = goToIndex < 0 ? 0 : goToIndex;

      listRef.current.children[index].scrollIntoView({
        behavior: 'smooth',
        block: "nearest",
        inline: "nearest"
      });
    }
  }

  const showHideNavArrows = () => {
    if (listRef.current.children && listRef.current.children.length > 0) {
      var listBoundings = listRef.current.getBoundingClientRect();
      setHideBackArrow(isElementVisible(listRef.current.children[0], listBoundings));
      setHideForwardArrow(isElementVisible(listRef.current.children[listRef.current.children.length - 1], listBoundings));
    }
  }

  return (
    <div className={"medias-list-container"}>
      {title ? <h2 className={`media-list-title ${centerTitle ? 'center' : ''}`}>{title}</h2> : null}
      <div ref={leftArrowRef} className="floating-navigation-btn left" onClick={() => navToLeft()}>
        <img alt="left" src={SemiArrowLeft}></img>
      </div>
      <div ref={rightArrowRef} className="floating-navigation-btn right" onClick={() => navToRight()}>
        <img alt="left" src={SemiArrowRight}></img>
      </div>
      <div ref={listRef} className={`medias-list ${hideBackArrow && hideForwardArrow ? 'center' : ''}`}>
        {medias.map((media, index) => (<MediaLitePresentation key={index} media={media} onMediaClick={() => onMediaClick(media)} />))}
      </div>
    </div>
  );
}
export default MediasHorizontalList;