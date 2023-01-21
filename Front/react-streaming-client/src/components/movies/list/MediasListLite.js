import "../../../style/medias-list.css";
import "../../../style/button.css";

import MediaLitePresentation from "../presentation/MediaLitePresentation";

import ArrowForwardIosRounded from '@material-ui/icons/ArrowForwardIosRounded';
import ArrowBackIosRounded from '@material-ui/icons/ArrowBackIosRounded';

import fadeTransition from "../../../js/customStyles.js";
import { useEffect, useRef, useState } from 'react';

function MediasListLite({ medias, onMediaClick }) {
  const listRef = useRef(null);
  const [hideBackArrow, setHideBackArrow] = useState(true);
  const [hideForwardArrow, setHideForwardArrow] = useState(false);

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

  useEffect(()=>{ showHideNavArrows(); }, [hideBackArrow, hideForwardArrow]);

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
      <div style={fadeTransition(!hideBackArrow)} className="floating-navigation-btn left" onClick={() => navToLeft()}>
        <ArrowBackIosRounded style={{ fontSize: 40, color: 'white' }} />
      </div>
      <div style={fadeTransition(!hideForwardArrow)} className="floating-navigation-btn right" onClick={() => navToRight()}>
        <ArrowForwardIosRounded style={{ fontSize: 40, color: 'white' }} />
      </div>
      <div ref={listRef} className={"medias-list"+ (hideBackArrow && hideForwardArrow ? ' center' : '')}>
        {medias.map((media, index) => (<MediaLitePresentation key={index} media={media} hoverEffect={true} onMediaClick={(mediaId)=>onMediaClick(mediaId)}/>))}
      </div>
    </div>
  );
}
export default MediasListLite;