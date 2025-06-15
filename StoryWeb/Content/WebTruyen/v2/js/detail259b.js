const detailScreen = {
    classChapterList: "main > .main-wrap #content .chapter-list .list",
    classRating: ".v-rating > div",
    classLoadMore: ".load-all > .btn-load-more",
    classStartChapter: ".start-chapter",
    classBtnFollow: ".btn-follow",
    classBtnAlert: ".btn-alert",
    classActiveFollow: "active-follow",
    classTxtFollowBtn: ".v-btn-content .text-capitalize",
    classDisabled: "text--disabled",
    btnSwitchType: ".btn-switch-type",
    initState: () => {
        detailScreen.initStateFollow();
        commonJS.initReadChapter(comic.nameEn);
        let trackingChapter = JSON.parse(tracking.get());
        if (!!trackingChapter) {
            const oldChapter = trackingChapter[comic.nameEn];
            if (!!oldChapter) {
                const url = messages.viewComic + oldChapter;
                const btn = $(detailScreen.classStartChapter);
                const htmlPrevBtn = `<a href="` + url + `" class="btn-recent mx-1 rounded-sm zs-bg-3 mr-4 btn-default btn-default-is-elevated theme-dark btn-size-default">
                                        <span class="v-btn-content">
                                            <span class="text-capitalize">` + messages.continueRead + oldChapter + `</span>
                                        </span>
                                    </a>`
                $(btn).html(btn.html() + htmlPrevBtn);
            }
        }

        if (!!comic.msgError) {
            notification.show(comic.msgError, notification.Error);
        }
    },
    clickChapter: (chapter) => {
        doRedirect(messages.viewComic + chapter);
    },
    startRead: (comicId) => {
        $.ajax({
            url: jsContext + `/api/comic/${comicId}/chapterNumber`,
            type: 'GET',
            dataType: "json"
        }).done(res => {
            if (res.status === true) {
                detailScreen.clickChapter(res.result.chapterNumbers[res.result.chapterNumbers.length - 1]);
            }
        })
    },
    doGo: () => {
        const chapter = $(".txt-go").val();
        if (!chapter) {
            notification.show(messages.emptyValue, notification.Warn);
            return;
        }
        doRedirect(messages.viewComic + chapter);
    },
    doSwitchType: (_self) => {
        const id = $(_self).attr("data-id");

        $.ajax({
            url: jsContext + `/api/novel/switch?type=novel&id=${id}`,
            beforeSend: beforeAuth,
            type: 'GET',
            dataType: "json"
        }).done(res => {
            if (res.status === true) {
                doRedirect(res.result.extraData);
            } else {
                notification.show(res.messages[0], notification.Info);
            }
        })
    },
    loadMore: (comicId, limit) => {
        $(".load-all").html(notification.addLoading());
        $.ajax({
            url: jsContext + `/api/comic/${comicId}/chapter?offset=${limit}&limit=-1`,
            type: 'GET',
            dataType: "json"
        }).done(res => {
            if (res.status === true) {
                const tbody = $(detailScreen.classChapterList);
                var html = $(tbody).html();
                const chapters = res.result.chapters;
                for (index in chapters) {
                    html += detailScreen.buildHtmlChapter(chapters[index]);
                }
                $(tbody).html(html);
                $(".load-all").hide();
                commonJS.initReadChapter(comic.nameEn);
            }
        })
    },
    doEvaluate: (_self) => {
        const rated = $(_self).parent().hasClass("rated");
        if (rated) {
            notification.show(messages.ratedChapter, notification.Info);
            return;
        }
        const score = parseInt($(_self).attr("data-score"));
        $.ajax({
            url: jsContext + `/api/comic/evaluate`,
            beforeSend: beforeAuth,
            type: 'POST',
            dataType: "json",
            data: {score: score, comicId: comic.id}
        }).done(res => {
            if (res.status === true) {
                notification.show(res.result.message, notification.Success);
                $(_self).parent().addClass("rated");
            } else {
                if (res.messages[0] === messages.ratedChapter) {
                    $(_self).parent().addClass("rated");
                }
                notification.show(res.messages[0], notification.Info);
            }
        })
    },
    initStateFollow: () => {
        $.ajax({
            url: jsContext + `/api/comic/follow/status`,
            beforeSend: beforeAuth,
            type: 'POST',
            dataType: "json",
            data: {comicId: comic.id, followSessionId : commonJS.getLocalSession()}
        }).done(res => {
            if (res.status) {
                if (res.result.state) {
                    $(detailScreen.classBtnFollow).addClass(detailScreen.classActiveFollow);
                    $(detailScreen.classBtnFollow).find(detailScreen.classTxtFollowBtn).html(messages.unfollow);
                }
                commonJS.storeLocalSession(res.result.extraData)
            }
        });
    },
    doFollow: (_self, push = false) => {
        if ($(_self).hasClass(detailScreen.classDisabled)) {
            return;
        }
        const isFollow = $(_self).hasClass(detailScreen.classActiveFollow);
        const url = jsContext + `/api/comic/` + (isFollow ? "unfollow" : "follow");
        $(_self).addClass(detailScreen.classDisabled);
        $.ajax({
            url: url,
            beforeSend: beforeAuth,
            type: 'POST',
            dataType: "json",
            data: {comicId: comic.id, push: push, followSessionId : commonJS.getLocalSession()}
        }).done(res => {
            if (res.status === true) {
                if (res.result.wait) {
                    notification.show(res.result.message, notification.Warn);
                    $(_self).addClass(detailScreen.classDisabled);
                    setTimeout(() => $(_self).removeClass(detailScreen.classDisabled), res.result.duration);
                } else {
                    notification.show(res.result.message, notification.Success);
                    if (isFollow) {
                        $(_self).removeClass(detailScreen.classActiveFollow);
                        $(_self).find(detailScreen.classTxtFollowBtn).html(messages.follow);
                    } else {
                        $(_self).addClass(detailScreen.classActiveFollow);
                        $(_self).find(detailScreen.classTxtFollowBtn).html(messages.unfollow);
                    }
                    $(_self).removeClass(detailScreen.classDisabled);
                }
            } else {
                notification.show(res.messages[0], notification.Warn);
                $(_self).removeClass(detailScreen.classDisabled);
            }
        }).catch(e => {
            $(_self).removeClass(detailScreen.classDisabled);
        })
    },
    processFollow: (self) => {
        if (!commonJS.isAuthenticated()) {
            notification.show(commonMessages.requiredLogin, notification.Info);
            return;
        }
        const isFollow = $(self).hasClass(detailScreen.classActiveFollow) || $(self).hasClass(detailScreen.classDisabled);
        try {
            const status = Notification.permission;
            const isDefault = status === "default";
            if (!isFollow) {
                let addFunc = undefined;
                if (isDefault) {
                    addFunc = (state, res) => {
                        notification.show(commonMessages.pushAllow, notification.Info);
                    };
                }
                registerPushNotification((token) => {
                    return $.ajax({
                        url: jsContext + `/api/push/add`,
                        beforeSend: beforeAuth,
                        type: 'POST',
                        dataType: "json",
                        data: {comicId: comic.id, token: token}
                    });
                }, addFunc);
            }

            // Do default
            if (status === "denied" || isDefault) {
                detailScreen.doFollow(self);
            } else {
                detailScreen.doFollow(self, true);
            }
        } catch (e) {
            //Error on ios
            detailScreen.doFollow(self, true);
        }
    },
    processPushMessage: (e) => {
        if (!commonJS.isAuthenticated()) {
            notification.show(commonMessages.requiredLogin, notification.Info);
            return;
        }
        try {
            e.stopPropagation();
            const isClick = $(e.target).hasClass(detailScreen.classDisabled);
            const status = Notification.permission;
            if (status === "denied") {
                notification.show(commonMessages.pushDenied, notification.Error);
                return;
            }
            if (!isClick) {
                registerPushNotification((token) => {
                    return $.ajax({
                        url: jsContext + `/api/push/add`,
                        beforeSend: beforeAuth,
                        type: 'POST',
                        dataType: "json",
                        data: {comicId: comic.id, token: token}
                    });
                }, (state, res) => {
                    if (res.status) {
                        if (res.result.state) {
                            notification.show(commonMessages.pushAllow, notification.Success);
                            $(e.target).addClass(detailScreen.classDisabled);
                        } else {
                            notification.show(commonMessages.pushNoAllow, notification.Warn);
                        }
                    } else {
                        notification.show(res.messages[0], notification.Info);
                    }
                }, (state) => {
                    if (state === "denied") {
                        notification.show(commonMessages.pushDenied, notification.Info);
                    }
                });
            } else {
                notification.show(commonMessages.pushAccept, notification.Info);
            }
        } catch (e) {
            //Error on ios
            notification.show(commonMessages.pushUnSupport, notification.Info);
        }
    },
    moveRating: (_self) => {
        const score = parseInt($(_self).attr("data-score"));
        const items = $(detailScreen.classRating);
        for (var i = 1; i <= items.length; i++) {
            if (i == 1) {
                $(items[i - 1]).addClass("active").removeClass("inactive");
                continue;
            }
            if (i > score) {
                $(items[i - 1]).addClass("inactive").removeClass("active");
            } else {
                $(items[i - 1]).addClass("active").removeClass("inactive");
            }
        }
    },
    buildHtmlChapter: (data) => {
        var html = "";
        if (!data.display) {
            return html;
        }
        var url = messages.viewComic + data.numberChapter;

        html += `<div class="col-md-6 col-lg-4 col-sm-6 col-12 w-100 p-12 py-1 border-box">
                    <a href="` + url + `" class="d-flex align-center zs-bg-3 pa-1 v-card v-card-flat v-card-link v-sheet theme-dark ` + data.numberChapter + `">
                <div class="c-icon d-flex align-center justify-center rounded icon_wrapper v-sheet theme-dark">`

        if (data.type == 'DOUBLE') {
            html += `<span class="v-icon notranslate theme-dark v-icon-svg-limit" style="color: #f78000; caret-color: #f78000;">
                        <svg class="v-icon-svg v-icon-svg-limit" viewBox="0 0 24 24">
                            <path d="M17 10.43V2H7v8.43c0 .35.18.68.49.86l4.18 2.51-.99 2.34-3.41.29 2.59 2.24L9.07 22 12 20.23 14.93 22l-.78-3.33 2.59-2.24-3.41-.29-.99-2.34 4.18-2.51c.3-.18.48-.5.48-.86zm-4 1.8-1 .6-1-.6V3h2v9.23z"></path>
                        </svg>
                        <span style="font-size: 14px">2+</span>
                    </span>`;
        } else {
            html += `<span class="v-icon notranslate theme-dark v-icon-svg-med" style="color: var(--primary); caret-color: var(--primary);">
                        <svg viewBox="0 0 24 24" class="v-icon-svg v-icon-svg-med">
                            <path d="M12 4.5A11.8 11.8 0 0 0 1 12A11.8 11.8 0 0 0 12 19.5H13.1A3.8 3.8 0 0 1 13 18.5A4.1 4.1 0 0 1 13.1 17.4H12A9.6 9.6 0 0 1 3.2 12A9.6 9.6 0 0 1 12 6.5A9.6 9.6 0 0 1 20.8 12L20.4 12.7A4.6 4.6 0 0 1 22.3 13.5A10.1 10.1 0 0 0 23 12A11.8 11.8 0 0 0 12 4.5M12 9A3 3 0 1 0 15 12A2.9 2.9 0 0 0 12 9M15 17.5V19.5H23V17.5Z"></path>
                        </svg>
                    </span>`;
        }
        let chapterName = "";
        if (data.name !== "N/A") {
            chapterName = "<span class='d-650-none'>: " + data.name + "</span>";
        }
        html += `</div>
                <div class="d-flex justify-center flex-column w-100 py-1 px-2">
                    <div class="chapter-info d-flex justify-space-between">
                        <div><span>#` + data.numberChapter + `</span>` + chapterName + `</div>
                    </div>
                    <div class="d-flex justify-space-between text--disabled text-caption">
                        <div class="d-flex"><div>` + data.stringUpdateTime + `</div></div>
                        <div class="text-caption">
                            <span class="v-icon notranslate theme-dark grey--text text--darken-1 v-icon-svg-small" >
                                <svg viewBox="0 0 24 24"class="v-icon-svg v-icon-svg-small">
                                    <path d="M12,9A3,3 0 0,1 15,12A3,3 0 0,1 12,15A3,3 0 0,1 9,12A3,3 0 0,1 12,9M12,4.5C17,4.5 21.27,7.61 23,12C21.27,16.39 17,19.5 12,19.5C7,19.5 2.73,16.39 1,12C2.73,7.61 7,4.5 12,4.5M3.18,12C4.83,15.36 8.24,17.5 12,17.5C15.76,17.5 19.17,15.36 20.82,12C19.17,8.64 15.76,6.5 12,6.5C8.24,6.5 4.83,8.64 3.18,12Z"></path>
                                </svg>
                            </span><span> ` + formattedNumber.format(data.viewCount) + `</span>
                        </div>
                    </div>
                </div>
            </a>
        </div>`;
        return html;
    }
}

$(document).ready(function() {
    detailScreen.initState();

    $(document).on("click", ".scroll-top", function() {
        detailScreen.scrollTop();
    });

    $(document).on("click", ".btn-start", function() {
        detailScreen.startRead(comic.id);
    });
    $(document).on("click", ".btn-go", function() {
        detailScreen.doGo();
    });
    $(document).on("keypress", ".txt-go", (e) => {
        if (e.keyCode === 13) {
            detailScreen.doGo();
        }
    });
    $(document).on("click", detailScreen.classLoadMore, function() {
        detailScreen.loadMore(comic.id, comic.limit);
    });
    setTimeout(() => {
        $(document).on("mouseover", detailScreen.classRating, function () {
            detailScreen.moveRating(this);
        });
    }, 2000);
    $(document).on("click", detailScreen.classRating, function () {
        detailScreen.doEvaluate(this);
    })
    $(document).on("click", detailScreen.classBtnFollow, function () {
        detailScreen.processFollow(this)
    });
    $(document).on("click", detailScreen.classBtnAlert, function (e) {
        detailScreen.processPushMessage(e)
    });
    $(document).on("click", detailScreen.btnSwitchType, function (e) {
        detailScreen.doSwitchType(e.target)
    });
});