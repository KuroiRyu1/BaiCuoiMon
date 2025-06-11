const HomeScreen = {
    classRecommendSection: ".recommend-section",
    classNewSection: ".new-section",
    classFollowSection: ".follow-recent-section",
    btnPrevRecommend: ".prev-recommend",
    btnNextRecommend: ".next-recommend",
    btnPrevNew: ".prev-new",
    btnNextNew: ".next-new",
    btnPrev: ".comic-list .prev",
    classLoading: ".comic-list .items",
    classRecommendLoading: ".recommend-section .comic",
    classNewLoading: ".new-section .comic",
    btnPrevStart: ".comic-list .prev svg",
    btnNext: ".comic-list .next",
    btnNextStart: ".comic-list .next svg",
    btnFollow: ".btn-filter-follow",
    btnFollowMore: ".btn-follow-load-more",
    btnHistory: ".btn-filter-history",
    homeConfiguration: "#home-filter-config",
    homeRecommendConfiguration: "#home-recommend-config",
    homeNewConfiguration: "#home-new-config",
    htmlItemCard: ".comic-list .items .item-card",
    btnHomeFilter: ".comic-list .btn-filter",
    btnArrowFilterRecent: ".comic-list .next-prev-recent",
    btnHomeLoadRecent: ".comic-list .btn-load-more-home",
    btnHomeRecommendFilter: ".recommend-section .btn-tab-filter",
    classBtnActiveFilter: ".btn-filter.text-active",
    classActiveFilter: "text-active",
    classComicList: ".comic-list",
    classDisabledArrow: "text--disabled",
    classShowLoading: "cover-items",
    classBoxFollow: ".follow-recent-section .box-follow",
    classFollowPanel: ".follow-recent-section  .follow-panel",
    classHideItems: "hide-items",
    classHideBox: "hide-box",
    cacheData: {},
    dataHistory: "",
    initState: () => {
        if (!!comic.msgError) {
            notification.show(comic.msgError, notification.Error);
        }
        HomeScreen.initializeFilter();
        $("img.lazy").lazyload({threshold : 200, effect : "fadeIn"});
        HomeScreen.trackUser(window.innerWidth);
        commonJS.initAllReadComic();
    },
    initializeFilter: () => {
        if (commonJS.isAuthenticated()) {
            // $(HomeScreen.btnFollow).addClass("filter-active");
            HomeScreen.fetchFollowRecent();
        }

        const historyData = JSON.parse(commonJS.getLocal("history_info"));
        if (!!historyData) {
            $(HomeScreen.btnHistory).addClass("filter-active");
            const len = historyData.length;
            for (let i = 0; i < len; i++) {
                HomeScreen.dataHistory += historyData[i].nameEn;
                if (i !== len - 1) {
                    HomeScreen.dataHistory += "|";
                }
            }
        }
    },
    trackUser: (width, append = "HOME", direct = false) => {
        $.ajax({
            url: jsContext + `/api/user/tracking`,
            beforeSend: beforeAuth,
            type: 'POST',
            dataType: "json",
            data: {
                width: width,
                name: !!localStorage.getItem("user_profile") + "|" + append,
                number: "0",
                direct: direct
            }
        }).done(res => {
            if (res.status && res.result) {
                localStorage.setItem("user_profile", "Empty");
            }
        })
    },
    handleResult: (result, page, type) => {
        $(HomeScreen.btnNextStart).removeClass(HomeScreen.classDisabledArrow);
        if (!result.next) {
            $(HomeScreen.btnNextStart).addClass(HomeScreen.classDisabledArrow);
        }
        HomeScreen.updatePaging(type, page);
        HomeScreen.drawItems(result.data);
    },
    handleRecommendAndNewResult: (result, page, type, btnNext, configuration, htmlSection) => {
        $(btnNext + " svg").removeClass(HomeScreen.classDisabledArrow);
        if (!result.next) {
            $(btnNext + " svg").addClass(HomeScreen.classDisabledArrow);
        }
        HomeScreen.updatePagingRecommendAndNew(type, page, configuration);
        HomeScreen.drawItemsRecommendAndNew(result.data, htmlSection);
    },
    doFilter: (page, type) => {
        const result = HomeScreen.cacheData[page + "-" + type];
        $(HomeScreen.classLoading).addClass(HomeScreen.classShowLoading);
        if (result) {
            HomeScreen.handleResult(result, page, type);
            $(HomeScreen.classLoading).removeClass(HomeScreen.classShowLoading);
            return;
        }

        if (type === "history") {
            const historyData = JSON.parse(localStorage.getItem("history_info"));
            let limit = parseInt($(HomeScreen.homeConfiguration).attr("data-limit"));
            let offset = parseInt(limit) * parseInt(page);
            let dataSplit = historyData.slice(offset, offset + limit);
            let data = [];
            for (let i = 0; i < dataSplit.length; i++) {
                let record = dataSplit[i];
                data[i] = {
                    chapterLatest: [record.chapter],
                    nameEn: record.nameEn,
                    chapterState: ["N/A"],
                    followerCount: "N/A",
                    viewCount: "N/A",
                    name: record.name,
                    photo: record.photo
                };
                try {
                    var dateParts = record.date.split(",")[0].split('/');
                    var day = parseInt(dateParts[1], 10);
                    if (day < 10) {
                        day = "0" + day;
                    }
                    var month = parseInt(dateParts[0], 10);
                    if (month < 10) {
                        month = "0" + month;
                    }
                    var year = parseInt(dateParts[2], 10);
                    data[i].chapterLatestDate = [day + "-" + month + "-" + year];
                } catch (ex) {
                    data[i].chapterLatestDate = [record.date];
                }
            }

            let resultHistory = {
                next: historyData.length > limit + offset,
                data: data
            }

            HomeScreen.handleResult(resultHistory, page, type);
            HomeScreen.cacheData[page + "-" + type] = resultHistory;
            $(HomeScreen.classLoading).removeClass(HomeScreen.classShowLoading);
            return;
        }

        $.ajax({
            url: jsContext + `/api/v2/home/filter`,
            beforeSend: beforeAuth,
            type: 'GET',
            dataType: "json",
            data: {
                p: page,
                value: type,
                extraData: type === "history" ? HomeScreen.dataHistory : ""
            }
        }).done(res => {
            if (res.status) {
                HomeScreen.handleResult(res.result, page, type);
                HomeScreen.cacheData[page + "-" + type] = res.result;
            }
            $(HomeScreen.classLoading).removeClass(HomeScreen.classShowLoading);
        });
    },
    doFilterNewAndRecommend: (page, type, classLoading, btnNext, homeConfiguration) => {
        const result = HomeScreen.cacheData[page + "-" + type];
        $(classLoading).addClass(HomeScreen.classShowLoading);
        if (result) {
            HomeScreen.handleRecommendAndNewResult(result, page, type, btnNext, homeConfiguration, classLoading);
            $(classLoading).removeClass(HomeScreen.classShowLoading);
            return;
        }
        $.ajax({
            url: jsContext + `/api/v2/home/filter`,
            beforeSend: beforeAuth,
            type: 'GET',
            dataType: "json",
            data: {
                p: page,
                value: type
            }
        }).done(res => {
            if (res.status) {
                HomeScreen.handleRecommendAndNewResult(res.result, page, type, btnNext, homeConfiguration, classLoading);
                HomeScreen.cacheData[page + "-" + type] = res.result;
            }
            $(classLoading).removeClass(HomeScreen.classShowLoading);
        });
    },
    drawItems: (data) => {
        if (data.length <= 0) {
            return;
        }

        let htmlData = "";
        for (const index in data) {
            htmlData += HomeScreen.buildCardItemHTML(data[index]);
        }
        $(HomeScreen.htmlItemCard).html(htmlData);
    },
    drawItemsRecommendAndNew: (data, clazzHtml) => {
        if (data.length <= 0) {
            return;
        }

        let htmlData = "";
        for (const index in data) {
            htmlData += HomeScreen.buildHTMLPanelComic(data[index]);
        }
        $(clazzHtml).html(htmlData);
    },
    updatePaging: (type, value) => {
        if (type === "follow") {
            $(HomeScreen.homeConfiguration).attr("data-follow", value);
            return;
        }
        if (type === "history") {
            $(HomeScreen.homeConfiguration).attr("data-history", value);
            return;
        }
        $(HomeScreen.homeConfiguration).attr("data-all", value);
    },
    updatePagingRecommendAndNew: (type, value, configuration) => {
        if (type === "recommend") {
            $(configuration).attr("data-recommend", value);
            return;
        }
        if (type === "favorite") {
            $(configuration).attr("data-favorite", value);
            return;
        }
        if (type === "new") {
            $(configuration).attr("data-new", value);
            return;
        }
    },
    progressPage: (_self, value) => {
        const type = $(HomeScreen.homeConfiguration).val();
        let page = $(HomeScreen.homeConfiguration).attr("data-all");
        if (type === "follow") {
            page = $(HomeScreen.homeConfiguration).attr("data-follow");
        }
        if (type === "history") {
            page = $(HomeScreen.homeConfiguration).attr("data-history");
        }
        if (parseInt(page) + value <= 0) {
            value = 0;
            page = 0;
        }
        if (value === 0) {
            $(HomeScreen.btnPrevStart).addClass(HomeScreen.classDisabledArrow);
        } else {
            $(HomeScreen.btnPrevStart).removeClass(HomeScreen.classDisabledArrow);
        }
        HomeScreen.doFilter(parseInt(page) + value, type);
        $("html, body").animate({
            scrollTop: $(HomeScreen.classComicList).offset().top
        }, "slow");
    },
    progressNewAndRecommendPagePage: (_self, value, homeConfiguration, btnPrev, btnNext, clazzComicList, classloading) => {
        const type = $(homeConfiguration).val();
        let page = $(homeConfiguration).attr("data-recommend");
        if (type === "favorite") {
            page = $(homeConfiguration).attr("data-favorite");
        }

        if (type === "new") {
            page = $(homeConfiguration).attr("data-new");
        }

        if (parseInt(page) + value <= 0) {
            value = 0;
            page = 0;
        }
        if (value === 0) {
            $(btnPrev + " svg").addClass(HomeScreen.classDisabledArrow);
        } else {
            $(btnPrev + " svg").removeClass(HomeScreen.classDisabledArrow);
        }
        HomeScreen.doFilterNewAndRecommend(parseInt(page) + value, type, classloading, btnNext, homeConfiguration);
        $("html, body").animate({
            scrollTop: $(clazzComicList).offset().top
        }, "slow");
    },
    fetchFollowRecent: () => {
        $.ajax({
            url: jsContext + `/api/comic/follower`,
            beforeSend: beforeAuth,
            type: 'GET',
            dataType: "json"
        }).done(res => {
            if (res.status) {
                const data = res.result.comics;
                if (res.result.next) {
                    $(HomeScreen.btnFollowMore).show();
                }
                if (data.length > 0) {
                    for (const index in data) {
                        $(HomeScreen.classBoxFollow).append(HomeScreen.buildCardFollowComic(data[index]));
                    }
                    if (!window.isMobile) {
                        $(HomeScreen.classFollowSection).removeClass(HomeScreen.classHideItems);
                    }
                    if (isWindowUser === "true") {
                        $(HomeScreen.classFollowSection).removeClass(HomeScreen.classHideItems);
                    }
                    $(HomeScreen.classFollowSection).removeClass(HomeScreen.classHideBox);
                }
            }
        });
    },
    buildCardFollowComic: (data) => {
        let html = "";
        html += `<div data-href="` + messages.routerDetail.replace("{0}", data.nameEn) + `" class="box-item border-box col-md-3-5 col-lg-3-5 col-sm-3-5 col-11 d-flex zs-bg-3 rounded ma-1">
                    <div class="box-image">
                        <img class="rounded" src="` + data.photo + `" alt="`+ data.photo  +`" />
                    </div>
                    <div class="box-content">
                        <div class="box-title">` + data.name + `</div>
                        <div data-href="` + messages.routerView.replace("{0}", data.nameEn) + data.chapterLatest[0] + `" class="box-chapter">` + messages.fieldChapter + " " + data.chapterLatest[0] +  `</div>
                    </div>
                </div>`;
        return html;
    },
    buildCardItemHTML: (data) => {
        let htmlChapter = "";
        for (i = 0; i < data.chapterLatest.length; i++) {
            let chapter = data.chapterLatest[i];
            htmlChapter += `<a href='` + messages.routerView.replace("{0}", data.nameEn) + chapter + `' title="` + messages.fieldChapter + " " + chapter + `" class="` + data.nameEn + `-` + chapter +` timeline-item mt-2 font-sans-serif">
                                    <div class="timeline-divider">
                                        <div class="timeline-item-dot">
                                            <div class="timeline-item-dot-inner new">
                                                <span  class="v-icon theme-dark">
                                                    <svg viewBox="0 0 24 24"  class="v-icon-svg-small">
                                                        <path d="M17.66 11.2C17.43 10.9 17.15 10.64 16.89 10.38C16.22 9.78 15.46 9.35 14.82 8.72C13.33 7.26 13 4.85 13.95 3C13 3.23 12.17 3.75 11.46 4.32C8.87 6.4 7.85 10.07 9.07 13.22C9.11 13.32 9.15 13.42 9.15 13.55C9.15 13.77 9 13.97 8.8 14.05C8.57 14.15 8.33 14.09 8.14 13.93C8.08 13.88 8.04 13.83 8 13.76C6.87 12.33 6.69 10.28 7.45 8.64C5.78 10 4.87 12.3 5 14.47C5.06 14.97 5.12 15.47 5.29 15.97C5.43 16.57 5.7 17.17 6 17.7C7.08 19.43 8.95 20.67 10.96 20.92C13.1 21.19 15.39 20.8 17.03 19.32C18.86 17.66 19.5 15 18.56 12.72L18.43 12.46C18.22 12 17.66 11.2 17.66 11.2M14.5 17.5C14.22 17.74 13.76 18 13.4 18.1C12.28 18.5 11.16 17.94 10.5 17.28C11.69 17 12.4 16.12 12.61 15.23C12.78 14.43 12.46 13.77 12.33 13C12.21 12.26 12.23 11.63 12.5 10.94C12.69 11.32 12.89 11.7 13.13 12C13.9 13 15.11 13.44 15.37 14.8C15.41 14.94 15.43 15.08 15.43 15.23C15.46 16.05 15.1 16.95 14.5 17.5H14.5Z"></path>
                                                        <path style="color: var(--primary-darken);" d="M17 19V22H15V19C15 17.9 14.1 17 13 17H10C7.2 17 5 14.8 5 12C5 10.8 5.4 9.8 6.1 8.9C3.8 8.5 2 6.4 2 4C2 3.3 2.2 2.6 2.4 2H4.8C4.3 2.5 4 3.2 4 4C4 5.7 5.3 7 7 7H10V9C8.3 9 7 10.3 7 12S8.3 15 10 15H13C15.2 15 17 16.8 17 19M17.9 8.9C20.2 8.5 22 6.4 22 4C22 3.3 21.8 2.6 21.6 2H19.2C19.7 2.5 20 3.2 20 4C20 5.7 18.7 7 17 7H15.8C15.9 7.3 16 7.6 16 8C16 9.7 14.7 11 13 11V13C15.8 13 18 15.2 18 18V22H20V18C20 15.3 18.5 13 16.2 11.8C17.1 11.1 17.7 10.1 17.9 8.9Z"></path>
                                                    </svg>
                                                </span>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="timeline-info">
                                        <span class="number">
                                            <span class="short-tag">#</span>
                                            <span class="long-tag">
                                                ` + messages.fieldChapter + `
                                            </span>` + chapter;
            if (data.chapterState[i] === 'RAW') {
                htmlChapter += ` <span class="chapter-state">Raw</span>`;
            } else if (data.chapterState[i] === 'ENG') {
                htmlChapter += `<span class="chapter-state">
                                        <svg class="icon-limit v-icon-svg v-icon-svg-limit" viewBox="0 0 24 24">
                                            <path d="M17 10.43V2H7v8.43c0 .35.18.68.49.86l4.18 2.51-.99 2.34-3.41.29 2.59 2.24L9.07 22 12 20.23 14.93 22l-.78-3.33 2.59-2.24-3.41-.29-.99-2.34 4.18-2.51c.3-.18.48-.5.48-.86zm-4 1.8-1 .6-1-.6V3h2v9.23z"></path>
                                        </svg>
                                    </span>`;
            } else if (data.chapterState[i] === 'VIDEO') {
                htmlChapter += ` <span class="chapter-state">Video</span>`;
            }
            htmlChapter += `</span><div class="spacer"></div><span class="time pr-1">` + data.chapterLatestDate[i] + `</span></div></a>`;
        }

        let hasNovelHTML = "";
        if (data.novelId !== undefined && data.novelId !== null && data.novelId !== "") {
            hasNovelHTML = '<img class="has-novel" title="' + commonMessages.hasNovel  + '" alt="Novel" src=' + jsContext + '"/contents/images/novel.png">'
        }

        let html = `<div class="col-md-2 col-lg-2 col-xl-2 col-sm-3 col-4 col-mb w-100 p-6 border-box" title="` + data.name + `">
                        <div class="card ele-2 zs-bg-3 d-flex flex-column elevation-2">
                            <a href='` + messages.routerDetail.replace("{0}", data.nameEn) + `' class="card-block card-flat theme-dark">
                                <div class="card-image v-responsive theme-dark">` + hasNovelHTML + `
                                    <img class="v-image" src="` + data.photo + `" title="` + data.name + `" alt="` + data.name + `">
                                    <div class="info-comic" >
                                        <span class="item-info">
                                            <svg class="svg-icon-size" focusable="false" viewBox="0 0 24 24"  data-testid="RemoveRedEyeIcon"><path d="M12 4.5C7 4.5 2.73 7.61 1 12c1.73 4.39 6 7.5 11 7.5s9.27-3.11 11-7.5c-1.73-4.39-6-7.5-11-7.5zM12 17c-2.76 0-5-2.24-5-5s2.24-5 5-5 5 2.24 5 5-2.24 5-5 5zm0-8c-1.66 0-3 1.34-3 3s1.34 3 3 3 3-1.34 3-3-1.34-3-3-3z"></path></svg>
                                            ` + data.viewCount + `
                                            <svg class="svg-icon-size" focusable="false" viewBox="0 0 24 24"  tabindex="-1" title="DirectionsWalk" data-ga-event-category="material-icons" data-ga-event-action="click" data-ga-event-label="DirectionsWalk"><path d="M13.5 5.5c1.1 0 2-.9 2-2s-.9-2-2-2-2 .9-2 2 .9 2 2 2zM9.8 8.9L7 23h2.1l1.8-8 2.1 2v6h2v-7.5l-2.1-2 .6-3C14.8 12 16.8 13 19 13v-2c-1.9 0-3.5-1-4.3-2.4l-1-1.6c-.4-.6-1-1-1.7-1-.3 0-.5.1-.8.1L6 8.3V13h2V9.6l1.8-.7"></path></svg>
                                             ` + data.followerCount + `
                                        </span>
                                    </div>
                                </div>
                            </a>
                            <div class="card-info">
                                <a href='` + messages.routerDetail.replace("{0}", data.nameEn) + `' class="card-name font-sans-serif px-2  mt-1">
                                    <span class="name">` + data.name + `</span>
                                </a>
                                <div class="card-chapters d-flex flex-column w-100 justify-space-evenly">
                                    <div class="card-timeline">
                                        <div class="timeline theme-dark">` +
            htmlChapter +
            `</div>
                                    </div>
                                </div>
                                <div class="info-comic">
                                    <span class="item-info">
                                        <svg class="svg-icon-size" focusable="false" viewBox="0 0 24 24" ><path d="M12 4.5C7 4.5 2.73 7.61 1 12c1.73 4.39 6 7.5 11 7.5s9.27-3.11 11-7.5c-1.73-4.39-6-7.5-11-7.5zM12 17c-2.76 0-5-2.24-5-5s2.24-5 5-5 5 2.24 5 5-2.24 5-5 5zm0-8c-1.66 0-3 1.34-3 3s1.34 3 3 3 3-1.34 3-3-1.34-3-3-3z"></path></svg>
                                        ` + data.viewCount + `
                                        <svg class="svg-icon-size" focusable="false" viewBox="0 0 24 24" tabindex="-1" ><path d="M13.5 5.5c1.1 0 2-.9 2-2s-.9-2-2-2-2 .9-2 2 .9 2 2 2zM9.8 8.9L7 23h2.1l1.8-8 2.1 2v6h2v-7.5l-2.1-2 .6-3C14.8 12 16.8 13 19 13v-2c-1.9 0-3.5-1-4.3-2.4l-1-1.6c-.4-.6-1-1-1.7-1-.3 0-.5.1-.8.1L6 8.3V13h2V9.6l1.8-.7"></path></svg>
                                        ` + data.followerCount + `
                                    </span>
                                </div>
                            </div>
                        </div>
                    </div>`;
        return html;
    },
    buildHTMLPanelComic: (data) => {
        const description = data.description.replaceAll("<", "&lt;").replaceAll(">", "&gt;");
        const rating = parseInt(data.evaluationScore);
        let htmlCategory = "";
        let lengthCategory = data.category.length;
        if (lengthCategory > 2) {
            lengthCategory = 2
        }
        for (let i = 0; i < lengthCategory; i++) {
            htmlCategory += `<a href="` + messages.routerSearchCategory.replace("{0}", data.categoryCode[i]) + `" class="mr-1 v-chip-active v-chip v-chip-clickable v-chip-link v-chip-no-color theme-dark v-size--x-small">
                                    <div class="v-chip-content">
                                        <span class="v-icon v-icon-left theme-dark">
                                            <svg viewBox="0 0 24 24"  class="v-icon-svg-small" >
                                                <path d="M5.5,7A1.5,1.5 0 0,1 4,5.5A1.5,1.5 0 0,1 5.5,4A1.5,1.5 0 0,1 7,5.5A1.5,1.5 0 0,1 5.5,7M21.41,11.58L12.41,2.58C12.05,2.22 11.55,2 11,2H4C2.89,2 2,2.89 2,4V11C2,11.55 2.22,12.05 2.59,12.41L11.58,21.41C11.95,21.77 12.45,22 13,22C13.55,22 14.05,21.77 14.41,21.41L21.41,14.41C21.78,14.05 22,13.55 22,13C22,12.44 21.77,11.94 21.41,11.58Z"></path>
                                            </svg>
                                        </span>
                                        <span>` + data.category[i] + `</span>
                                    </div>
                                </a>`
        }

        let html = `<div class="lds-loading"><div></div><div></div><div></div></div><div class="col-md-6 col-lg-6 col-12 w-100 p-12 border-box" title="` + data.name + `">
                            <div class="block-comic">
                                <div class="info2 d-flex align-center w-100 text--disabled d-lx-none border-box" >
                                    <div class="text-caption">` + data.chapterLatest[0] + ` ` + messages.fieldChapter + `</div>
                                    <div class="spacer" ></div>
                                    <div class="d-flex g-10">
                                        <div>
                                            <span aria-hidden="true" class="display-inline-block v-icon-2 theme-dark grey--text text--darken-1" >
                                                <svg  viewBox="0 0 24 24" aria-hidden="true" class="v-icon-svg-med yellow-color" >
                                                    <path d="M12,17.27L18.18,21L16.54,13.97L22,9.24L14.81,8.62L12,2L9.19,8.62L2,9.24L7.45,13.97L5.82,21L12,17.27Z"></path>
                                                </svg>
                                            </span>
                                            <span class="text-caption">` + rating + `</span>
                                        </div>
                                        <div>
                                            <span aria-hidden="true" class="display-inline-block v-icon-2 theme-dark grey--text text--darken-1" >
                                                <svg  viewBox="0 0 24 24" aria-hidden="true" class="v-icon-svg-med" >
                                                    <path d="M12,9A3,3 0 0,0 9,12A3,3 0 0,0 12,15A3,3 0 0,0 15,12A3,3 0 0,0 12,9M12,17A5,5 0 0,1 7,12A5,5 0 0,1 12,7A5,5 0 0,1 17,12A5,5 0 0,1 12,17M12,4.5C7,4.5 2.73,7.61 1,12C2.73,16.39 7,19.5 12,19.5C17,19.5 21.27,16.39 23,12C21.27,7.61 17,4.5 12,4.5Z"></path>
                                                </svg>
                                            </span>
                                        <span class="text-caption">` + data.viewCount + `</span>
                                        </div>
                                        <div>
                                            <span aria-hidden="true" class="display-inline-block v-icon-2 theme-dark grey--text text--darken-1" >
                                                <svg  viewBox="0 0 24 24" aria-hidden="true" class="v-icon-svg-med">
                                                    <path d="M13.5 5.5c1.1 0 2-.9 2-2s-.9-2-2-2-2 .9-2 2 .9 2 2 2zM9.8 8.9L7 23h2.1l1.8-8 2.1 2v6h2v-7.5l-2.1-2 .6-3C14.8 12 16.8 13 19 13v-2c-1.9 0-3.5-1-4.3-2.4l-1-1.6c-.4-.6-1-1-1.7-1-.3 0-.5.1-.8.1L6 8.3V13h2V9.6l1.8-.7"></path>
                                                </svg>
                                            </span>
                                            <span class="text-caption">` + data.followerCount + `</span>
                                        </div>
                                    </div>
                                </div>
                                <div class="block-wrap d-flex rounded zs-bg-3 ele-2">
                                    <div class="relative">
                                        <a href='` + messages.routerDetail.replace("{0}", data.nameEn) + `' title="` + data.name + `">
                                            <img class="image" src="` + data.photo + `" />
                                        </a>
                                    </div>
                                    <div class="info py-2">
                                        <div class="px-4 align-center w-100 text--disabled d-lx-flex d-none border-box" >
                                            <div class="text-caption">` + data.chapterLatest[0] + ` ` + messages.fieldChapter + `</div>
                                            <div class="spacer" ></div>
                                            <div class="d-flex g-10" >
                                                <div>
                                                    <span aria-hidden="true" class="display-inline-block v-icon-2 theme-dark grey--text text--darken-1" >
                                                        <svg  viewBox="0 0 24 24" aria-hidden="true" class="v-icon-svg-med yellow-color" >
                                                            <path d="M12,17.27L18.18,21L16.54,13.97L22,9.24L14.81,8.62L12,2L9.19,8.62L2,9.24L7.45,13.97L5.82,21L12,17.27Z"></path>
                                                        </svg>
                                                    </span>
                                                    <span class="text-caption">` + rating + `</span>
                                                </div>
                                                <div>
                                                    <span aria-hidden="true" class="display-inline-block v-icon-2 theme-dark grey--text text--darken-1" >
                                                        <svg  viewBox="0 0 24 24" aria-hidden="true" class="v-icon-svg-med" >
                                                            <path d="M12,9A3,3 0 0,0 9,12A3,3 0 0,0 12,15A3,3 0 0,0 15,12A3,3 0 0,0 12,9M12,17A5,5 0 0,1 7,12A5,5 0 0,1 12,7A5,5 0 0,1 17,12A5,5 0 0,1 12,17M12,4.5C7,4.5 2.73,7.61 1,12C2.73,16.39 7,19.5 12,19.5C17,19.5 21.27,16.39 23,12C21.27,7.61 17,4.5 12,4.5Z"></path>
                                                        </svg>
                                                    </span>
                                                    <span class="text-caption">` + data.viewCount + `</span>
                                                </div>
                                                <div>
                                                    <span aria-hidden="true" class="display-inline-block v-icon-2 theme-dark grey--text text--darken-1" >
                                                        <svg  viewBox="0 0 24 24" aria-hidden="true" class="v-icon-svg-med" >
                                                            <path d="M13.5 5.5c1.1 0 2-.9 2-2s-.9-2-2-2-2 .9-2 2 .9 2 2 2zM9.8 8.9L7 23h2.1l1.8-8 2.1 2v6h2v-7.5l-2.1-2 .6-3C14.8 12 16.8 13 19 13v-2c-1.9 0-3.5-1-4.3-2.4l-1-1.6c-.4-.6-1-1-1.7-1-.3 0-.5.1-.8.1L6 8.3V13h2V9.6l1.8-.7"></path>
                                                        </svg>
                                                    </span>
                                                    <span class="text-caption">` + data.followerCount + `</span>
                                                </div>
                                            </div>
                                        </div>
                                        <a href='` + messages.routerDetail.replace("{0}", data.nameEn) + `' title="` + data.name + `" class="mx-4 text-body-1 font-weight-bold btn-link-text d-block text-truncate">` + data.name + `</a>
                                        <div class="px-4 my-1 align-center" title="">` + htmlCategory + `</div>
                                        <div class="px-4 text-body-3 text-secondary line-clamp-3" title="` + description + `">` + description + `</div>
                                        <div class="px-2 pt-2" style="position: absolute;bottom: 10px;">
                                            <a href='` + messages.routerDetail.replace("{0}", data.nameEn) + `' class="v-btn v-btn-router v-btn-text theme-dark v-size-small" style="color: var(--primary); caret-color: var(--primary);">
                                                <span>` + messages.readBtn + `</span>
                                            </a>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>`;
        return html;
    }
}

$(document).ready(function() {
    HomeScreen.initState();

    $(document).on("click", HomeScreen.btnNext, function(e) {
        e.stopPropagation();
        if ($(HomeScreen.btnNextStart).hasClass(HomeScreen.classDisabledArrow)) {
            return;
        }
        HomeScreen.progressPage(e.target, 1);
    });
    $(document).on("click", HomeScreen.btnPrev, function(e) {
        e.stopPropagation();
        if ($(HomeScreen.btnPrevStart).hasClass(HomeScreen.classDisabledArrow)) {
            return;
        }
        HomeScreen.progressPage(e.target, -1);
    });
    $(document).on("click", HomeScreen.btnHomeFilter, function(e) {
        e.stopPropagation();
        $(HomeScreen.btnHomeFilter).removeClass(HomeScreen.classActiveFilter);
        $(e.target).closest(HomeScreen.btnHomeFilter).addClass(HomeScreen.classActiveFilter);
        const type = $(e.target).closest(HomeScreen.btnHomeFilter).attr("data-type");
        $(HomeScreen.homeConfiguration).val(type);
        let page = $(HomeScreen.homeConfiguration).attr("data-all");

        $(HomeScreen.btnHomeLoadRecent).show();
        $(HomeScreen.btnArrowFilterRecent).hide();
        if (type === "follow") {
            page = $(HomeScreen.homeConfiguration).attr("data-follow");
        }
        if (type === "history") {
            page = $(HomeScreen.homeConfiguration).attr("data-history");
            $(HomeScreen.btnHomeLoadRecent).hide();
            $(HomeScreen.btnArrowFilterRecent).show();
        }

        $(HomeScreen.btnPrevStart).removeClass(HomeScreen.classDisabledArrow);
        $(HomeScreen.btnNextStart).removeClass(HomeScreen.classDisabledArrow);
        page = parseInt(page);
        if (page === 0) {
            $(HomeScreen.btnPrevStart).addClass(HomeScreen.classDisabledArrow);
        }
        HomeScreen.doFilter(page, type);
    });
    $(document).on("click", HomeScreen.btnHomeRecommendFilter, function(e) {
        e.stopPropagation();
        $(HomeScreen.btnHomeRecommendFilter).removeClass(HomeScreen.classActiveFilter);
        const type = $(e.target).closest(HomeScreen.btnHomeRecommendFilter).addClass(HomeScreen.classActiveFilter).attr("data-type");
        $(HomeScreen.homeRecommendConfiguration).val(type);
        let page = $(HomeScreen.homeRecommendConfiguration).attr("data-recommend");

        if (type === "favorite") {
            page = $(HomeScreen.homeRecommendConfiguration).attr("data-favorite");
        }

        $(HomeScreen.btnPrevRecommend + " svg").removeClass(HomeScreen.classDisabledArrow);
        $(HomeScreen.btnNextRecommend + " svg").removeClass(HomeScreen.classDisabledArrow);
        page = parseInt(page);
        if (page === 0) {
            $(HomeScreen.btnPrevRecommend + " svg").addClass(HomeScreen.classDisabledArrow);
        }
        HomeScreen.doFilterNewAndRecommend(page, type, HomeScreen.classRecommendLoading, HomeScreen.btnNextRecommend, HomeScreen.homeRecommendConfiguration);
    });
    $(document).on("click", HomeScreen.btnNextRecommend, function(e) {
        e.stopPropagation();
        if ($(HomeScreen.btnNextRecommend + " svg").hasClass(HomeScreen.classDisabledArrow)) {
            return;
        }
        HomeScreen.progressNewAndRecommendPagePage(e.target, 1, HomeScreen.homeRecommendConfiguration, HomeScreen.btnPrevRecommend, HomeScreen.btnNextRecommend, HomeScreen.classRecommendSection, HomeScreen.classRecommendLoading);
    });
    $(document).on("click", HomeScreen.btnPrevRecommend, function(e) {
        e.stopPropagation();
        if ($(HomeScreen.btnPrevRecommend + " svg").hasClass(HomeScreen.classDisabledArrow)) {
            return;
        }
        HomeScreen.progressNewAndRecommendPagePage(e.target, -1, HomeScreen.homeRecommendConfiguration, HomeScreen.btnPrevRecommend, HomeScreen.btnNextRecommend, HomeScreen.classRecommendSection, HomeScreen.classRecommendLoading);
    });
    $(document).on("click", HomeScreen.btnNextNew, function(e) {
        e.stopPropagation();
        if ($(HomeScreen.btnNextNew + " svg").hasClass(HomeScreen.classDisabledArrow)) {
            return;
        }
        HomeScreen.progressNewAndRecommendPagePage(e.target, 1, HomeScreen.homeNewConfiguration, HomeScreen.btnPrevNew, HomeScreen.btnNextNew, HomeScreen.classNewSection, HomeScreen.classNewLoading);
    });
    $(document).on("click", HomeScreen.classBoxFollow + " .box-chapter", function(e) {
        e.stopPropagation();
        const url = $(e.target).attr("data-href");
        doRedirect(url);

    });

    $(document).on("click", HomeScreen.classBoxFollow, function(e) {
        e.stopPropagation();
        const url = $(e.target).closest(".box-item").attr("data-href");
        if (url === undefined) {
            return;
        }
        doRedirect(url);

    });
    $(document).on("click", HomeScreen.classFollowPanel, function(e) {
        if ($(HomeScreen.classFollowSection).hasClass(HomeScreen.classHideItems)) {
            $(HomeScreen.classFollowSection).removeClass(HomeScreen.classHideItems);
            return;
        }
        $(HomeScreen.classFollowSection).addClass(HomeScreen.classHideItems);
    });
    $(document).on("click", HomeScreen.btnPrevNew, function(e) {
        e.stopPropagation();
        if ($(HomeScreen.btnPrevNew + " svg").hasClass(HomeScreen.classDisabledArrow)) {
            return;
        }
        HomeScreen.progressNewAndRecommendPagePage(e.target, -1, HomeScreen.homeNewConfiguration, HomeScreen.btnPrevNew, HomeScreen.btnNextNew, HomeScreen.classNewSection, HomeScreen.classNewLoading);
    });
});
