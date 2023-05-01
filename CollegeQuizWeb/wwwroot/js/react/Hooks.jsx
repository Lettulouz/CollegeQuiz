const { useState } = React;

export const useLoadableContent = (spinnerId = 'react-loadable-spinner-content') => {
    const [ showContent, setShowContent ] = useState(false);

    const showContentCallback = () => {
        document.getElementById(spinnerId).style.cssText = 'display:none !important';
        setShowContent(true);
    };

    const hideContentCallback = () => {
        document.getElementById(spinnerId).style.cssText = 'display:flex !important';
        setShowContent(false);
    };

    return [ showContent, showContentCallback, hideContentCallback ];
};