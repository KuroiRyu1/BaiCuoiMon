const RecentScreen = {

    load: () => {
        $("img.lazy").lazyload({threshold : 200, effect : "fadeIn"});
        commonJS.initAllReadComic();

    }
}

$(document).ready(function () {
    RecentScreen.load();
});