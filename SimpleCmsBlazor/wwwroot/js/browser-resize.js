window.browserResize = {
    getInnerHeight: function () {
        return window.innerHeight;
    },
    getInnerWidth: function () {
        return window.innerWidth;
    },
    registerResizeCallback: function () {
        window.addEventListener("resize", browserResize.resized);
    },
    resized: function () {
        DotNet.invokeMethodAsync("BrowserResize", 'OnBrowserResize', window.innerWidth, window.innerHeight).then(data => data);
    }
}