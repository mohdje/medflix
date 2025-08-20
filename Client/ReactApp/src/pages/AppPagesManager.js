
import MediasListOfCategoriePage from "./MediasListOfCategoriePage.js";
import MediaPresentationPage from "./MediaPresentationPage";
import HomePage from "./HomePage";
import WatchedMediasPage from "./WatchedMediasPage";
import BookmarkedMediasPage from "./BookmarkedMediasPage";
import ResearchPage from "./ResearchPage";

import { useEffect, useState, useRef } from 'react';

export function AppPagesManager({ activePage, onHomeReady, onHomeFail }) {
    const [pageComponentsStack, setPageComponentsStack] = useState([]);
    const pageComponentsStackRef = useRef([]);

    useEffect(() => {
        const pageComponent = pages.find(page => page.id === activePage.id).component(activePage.parameter);
        pageComponentsStackRef.current = [{ component: pageComponent, scrollPosition: 0 }];
        setPageComponentsStack(pageComponentsStackRef.current.map(page => page.component));
    }, [activePage]);

    useEffect(() => {
        window.scrollBy(0, pageComponentsStackRef.current[pageComponentsStackRef.current.length - 1].scrollPosition);
    }, [pageComponentsStack]);

    const onMediaClick = (media) => {
        const pageComponent = pages.find(page => page.id === PageIds.mediaPresentationPage).component(media.id);
        pageComponentsStackRef.current[pageComponentsStackRef.current.length - 1].scrollPosition = window.scrollY;
        pageComponentsStackRef.current.push({ component: pageComponent, scrollPosition: 0 });
        setPageComponentsStack(pageComponentsStackRef.current.map(page => page.component));
    }

    const backToPreviousComponent = () => {
        pageComponentsStackRef.current.pop();
        setPageComponentsStack(pageComponentsStackRef.current.map(page => page.component));
    }

    const pages = [
        {
            id: PageIds.homePage,
            component: () => {
                return <HomePage
                    onReady={() => onHomeReady()}
                    onFail={() => onHomeFail()}
                    onMediaClick={(media) => onMediaClick(media)} />
            }
        },
        {
            id: PageIds.mediasListOfCategoriePage,
            component: (parameter) => {
                return <MediasListOfCategoriePage
                    categorie={parameter.categorie}
                    type={parameter.type}
                    onMediaClick={(media) => onMediaClick(media)} />
            },
        },
        {
            id: PageIds.watchedMediasListPage,
            component: () => {
                return <WatchedMediasPage
                    onMediaClick={(media) => onMediaClick(media)} />
            }
        },
        {
            id: PageIds.bookmarkedMediasListPage,
            component: () => {
                return <BookmarkedMediasPage
                    onMediaClick={(media) => onMediaClick(media)} />
            }
        },
        {
            id: PageIds.researchPage,
            component: () => {
                return <ResearchPage
                    onMediaClick={(media) => onMediaClick(media)} />
            }
        },
        {
            id: PageIds.mediaPresentationPage,
            component: (mediaId) => {
                return <MediaPresentationPage
                    mediaId={mediaId}
                    onSimilarMediaClick={(media) => onMediaClick(media)}
                    onCloseClick={() => { backToPreviousComponent() }} />
            }
        }];


    return <>
        {pageComponentsStack.map((pageComponent, index) =>
            <div key={index} style={{ display: index === pageComponentsStack.length - 1 ? '' : 'none' }}>
                {pageComponent}
            </div>
        )}
    </>;
}

export const PageIds = {
    homePage: 'homePage',
    mediasListOfCategoriePage: 'mediasOfGenre',
    mediasListPlatformPage: 'mediasOfPlatform',
    mediaPresentationPage: 'mediaPresentationPage',
    watchedMediasListPage: 'watchedMediasListPage',
    bookmarkedMediasListPage: 'bookmarkedMediasListPage',
    researchPage: 'researchPage'
}
