const SearchScreen = {
    clazzSection: ".search-section",
    clazzBlockItem: ".block-item",
    clazzBlockLegend: ".block-item legend",
    clazzItem: ".item-status",
    clazzActive: "active",
    clazzBlockStatus: ".block-status",
    clazzBlockCategory: ".block-category",
    clazzBlockOrder: ".block-order",
    clazzCheckBoxActive: ".active .cbox",
    clazzSearchValue: ".search-value",
    clazzBtnSearch: ".btn-search-action",
    clazzComics: ".search-comic .comic",
    clazzShowLoading: "cover-items",
    clazzSearchLoading: ".search-section .comic",
    clazzNextPage: ".search-section .next",
    clazzPrevPage: ".search-section .prev",
    clazzIdPaging: "#id-paging",
    clazzDisablePaging: "text--disabled",
    pendingProgress: undefined,
    initState: () => {
        let categoriesCheckBox = $(SearchScreen.clazzBlockCategory).find(SearchScreen.clazzCheckBoxActive);
        let orderCheckBox = $(SearchScreen.clazzBlockOrder).find(SearchScreen.clazzCheckBoxActive);
        if (categoriesCheckBox.length > 0 || orderCheckBox.length > 0) {
            SearchScreen.doSearch();
        }
    },
    toggleCheckbox: (self) => {
        if ($(self).hasClass(SearchScreen.clazzActive)) {
            $(self).removeClass(SearchScreen.clazzActive).find("svg").html(SearchScreen.htmlInactiveIcon());
        } else {
            $(self).addClass(SearchScreen.clazzActive).find("svg").html(SearchScreen.htmlActiveIcon());
        }
    },
    initPaging: () => {
        $(SearchScreen.clazzPrevPage).find("svg").removeClass(SearchScreen.clazzDisablePaging);
        $(SearchScreen.clazzNextPage).find("svg").removeClass(SearchScreen.clazzDisablePaging);
        $(SearchScreen.clazzPrevPage).find("svg").addClass(SearchScreen.clazzDisablePaging);
        $(SearchScreen.clazzNextPage).find("svg").addClass(SearchScreen.clazzDisablePaging);
        $("html, body").animate({
            scrollTop: $(SearchScreen.clazzSection).offset().top
        }, "slow");
    },
    toggleTab: (self) => {
        self = $(self).closest(SearchScreen.clazzBlockItem);
        if($(self).closest(SearchScreen.clazzBlockItem).hasClass("hide")) {
            $(self).removeClass("hide");
        } else {
            $(self).addClass("hide");
        }
    },
    doSearch: () => {
        if ($(SearchScreen.clazzSearchLoading).hasClass(SearchScreen.clazzShowLoading)) {
            return;
        }
        if (SearchScreen.pendingProgress !== undefined) {
            clearTimeout(SearchScreen.pendingProgress);
        }
        $(SearchScreen.clazzSearchLoading).removeClass(SearchScreen.clazzShowLoading)
        $(SearchScreen.clazzSearchLoading).addClass(SearchScreen.clazzShowLoading);
        SearchScreen.pendingProgress = setTimeout(() => {
            let status = [];
            let categories = [];
            let orders = [];
            let statusCheckBox = $(SearchScreen.clazzBlockStatus).find(SearchScreen.clazzCheckBoxActive)
            for (let i = 0; i < statusCheckBox.length; i++) {
                status.push($(statusCheckBox[i]).attr("data-code"));
            }
            let categoriesCheckBox = $(SearchScreen.clazzBlockCategory).find(SearchScreen.clazzCheckBoxActive)
            for (let i = 0; i < categoriesCheckBox.length; i++) {
                categories.push($(categoriesCheckBox[i]).attr("data-code"));
            }
            let ordersCheckBox = $(SearchScreen.clazzBlockOrder).find(SearchScreen.clazzCheckBoxActive)
            for (let i = 0; i < ordersCheckBox.length; i++) {
                orders.push($(ordersCheckBox[i]).attr("data-code"));
            }

            let page = parseInt($(SearchScreen.clazzIdPaging).val());

            SearchScreen.initPaging();

            $.ajax({
                url: jsContext + `/api/v2/search`,
                beforeSend: beforeAuth,
                type: 'GET',
                dataType: "json",
                data: {
                    p: page,
                    searchValue: $(SearchScreen.clazzSearchValue).val(),
                    status: status,
                    categories: categories,
                    orders: orders
                }
            }).done(res => {
                if (res.status) {
                    const data = res.result.data;
                    if (res.result.p != 0) {
                        $(SearchScreen.clazzPrevPage).find("svg").removeClass(SearchScreen.clazzDisablePaging);
                    }

                    if (res.result.next) {
                        $(SearchScreen.clazzNextPage).find("svg").removeClass(SearchScreen.clazzDisablePaging);
                    }

                    if (data.length <= 0) {
                        $(SearchScreen.clazzComics).html(`<div class="lds-loading"><div></div><div></div><div></div></div><div class="m-auto description  pt-4 pb-4" >` + messages.noResult + `</div>`);
                        $(SearchScreen.clazzSearchLoading).removeClass(SearchScreen.clazzShowLoading);
                        return;
                    }
                    let htmlData = `<div class="items-wrap app-window app-item-group theme-dark">
                                        <div class="app-window-container">
                                            <div class="items">
                                                <div class="lds-loading">
                                                    <div></div>
                                                    <div></div>
                                                    <div></div>
                                                </div>
                                                <div class="row item-card">`;
                    for (const index in data) {
                        htmlData += SearchScreen.buildCardItemHTML(data[index]);
                    }
                    htmlData += "</div></div></div></div>";
                    $(SearchScreen.clazzComics).html(htmlData);
                    commonJS.initAllReadComic();
                }
                $(SearchScreen.clazzSearchLoading).removeClass(SearchScreen.clazzShowLoading);
            });
        }, 500);
    },
    htmlActiveIcon: () => {
        return `<path d="M10,17L5,12L6.41,10.58L10,14.17L17.59,6.58L19,8M19,3H5C3.89,3 3,3.89 3,5V19C3,20.1 3.9,21 5,21H19C20.1,21 21,20.1 21,19V5C21,3.89 20.1,3 19,3Z"></path>`;
    },
    htmlInactiveIcon: () => {
        return `<path d="M19,3H5C3.89,3 3,3.89 3,5V19C3,20.1 3.9,21 5,21H19C20.1,21 21,20.1 21,19V5C21,3.89 20.1,3 19,3M19,5V19H5V5H19Z"></path>`;
    },
    buildCardItemHTML: (data) => {
        let htmlChapter = "";
        for (i = 0; i < data.chapterLatest.length; i++) {
            let chapter = data.chapterLatest[i];
            htmlChapter += `<a target="_blank" href='` + messages.routerView.replace("{0}", data.nameEn) + chapter + `' title="` + messages.fieldChapter + " " + chapter + `" class="` + data.nameEn + `-` + chapter +` timeline-item mt-2 font-sans-serif">
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
        if (data.novelId !== null && data.novelId !== "") {
            hasNovelHTML = '<img class="has-novel" title="' + commonMessages.hasNovel  + '" alt="Novel" src=' + jsContext + '"/contents/images/novel.png">'
        }

        let html = `<div class="col-md-2 col-lg-2 col-xl-2 col-sm-3 col-4 col-mb w-100 p-6 border-box card-reader" data-card="`+ data.nameEn +`" title="` + data.name + `">
                        <div class="card ele-2 zs-bg-3 d-flex flex-column elevation-2">
                            <a target="_blank" href='` + messages.routerDetail.replace("{0}", data.nameEn) + `' class="card-block card-flat theme-dark">
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
                                <a target="_blank" href='` + messages.routerDetail.replace("{0}", data.nameEn) + `' class="card-name font-sans-serif px-2  mt-1">
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
    }
}
$(document).ready(function () {
    SearchScreen.initState();
    $(document).on("click", SearchScreen.clazzItem, function (e) {
        e.stopPropagation();
        var target = $(e.target).closest(SearchScreen.clazzItem);
        SearchScreen.toggleCheckbox(target);
    });
    $(document).on("keypress", SearchScreen.clazzSearchValue + ",body", (e) => {
        if (e.keyCode === 13) {
            $(SearchScreen.clazzIdPaging).val(0);
            SearchScreen.doSearch();
        }
    });
    $(document).on("click", SearchScreen.clazzBtnSearch, function (e) {
        $(SearchScreen.clazzIdPaging).val(0);
        SearchScreen.doSearch();
    });
    $(document).on("click", SearchScreen.clazzPrevPage, function (e) {
        if ($(SearchScreen.clazzPrevPage).find("svg").hasClass(SearchScreen.clazzDisablePaging)) {
            return;
        }

        let page = parseInt($(SearchScreen.clazzIdPaging).val());
        page = page - 1
        if (page < 0) {
            page = 0
        }

        $(SearchScreen.clazzIdPaging).val(page);
        SearchScreen.doSearch();
    });
    $(document).on("click", SearchScreen.clazzNextPage, function (e) {
        if ($(SearchScreen.clazzNextPage).find("svg").hasClass(SearchScreen.clazzDisablePaging)) {
            return;
        }

        let page = parseInt($(SearchScreen.clazzIdPaging).val());
        page = page + 1;

        $(SearchScreen.clazzIdPaging).val(page);
        SearchScreen.doSearch();
    });
    $(document).on("click", SearchScreen.clazzBlockLegend, function (e) {
        SearchScreen.toggleTab(e.target);
    });
});