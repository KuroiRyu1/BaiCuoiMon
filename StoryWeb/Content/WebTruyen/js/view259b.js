const ViewComic = {
    active: "active",
    navListClass: ".nav-list",
    navNextClass: ".nav-next",
    navPrevClass: ".nav-prev",
    navReportClass: ".nav-report",
    dialogId: "#dialog",
    dialogReportId: "#dialog-report",
    btnCancelClass: "#dialog-report .btn-cancel",
    btnCloseClass: ".btn-close",
    btnCloseReport: ".btn-close-report",
    toggleSettingClass: ".setting > .toggle",
    settingClass: ".setting",
    switchServerClass: ".switch-server",
    inputSearchClass: ".input-search",
    btnChapterClass: "#dialog .btn-chapter",
    btnHomeSettingClass: ".nav-home",
    btnFollowClass: ".nav-follow",
    btnSendReportClass: ".btn-send",
    txtContentReport: ".txt-content-report",
    clazzImagesBlock: ".image-section .img-block img:not(.finished)",
    clazzImageBlock: ".img-block",
    clazzImageArea: ".main .main-wrap .main-images .image-section",
    clazzFixedClickPanel: ".fixed-click-panel",
    lastTapTime: 0,
    inputComment: ".comment-section .input-comment",
    keepTouch: undefined,
    doClickPanelOnImage: (event, isTouch = false) => {
        if ($(ViewComic.clazzFixedClickPanel).hasClass(ViewComic.active)) {
            $(ViewComic.clazzFixedClickPanel).removeClass(ViewComic.active);
            return;
        }
        let bufferX = 120;
        let bufferY = 148;
        let screenX = event.clientX;
        let screenY = event.clientY;
        if (isTouch) {
            if (event.changedTouches.length > 0) {
                screenX = event.changedTouches[0].clientX;
                screenY = event.changedTouches[0].clientY;
            } else {
                screenX = window.innerWidth / 2;
                screenY = window.innerHeight / 2;
            }

        }
        var posX = screenX - bufferX,
            posY = screenY - bufferY;
        $(ViewComic.clazzFixedClickPanel)
            .css("left", posX  + "px")
            .css("top", posY  + "px")
            .addClass(ViewComic.active);
    },
    doSwitchingServer: () => {
        const images = $(ViewComic.clazzImagesBlock);
        if (images.length < 1) {
            notification.show(messages.imagesLoaded, notification.Info);
            return;
        }
        const serverData = servers.backup.split(",");
        let currentServer = servers.current;
        let switchServer = servers.current;

        let urlSource = $(images[0]).attr("src");
        if (!urlSource.startsWith("http")) {
            urlSource = currentServer + urlSource;
        }
        for (let i = 0; i < serverData.length; i++) {
            let server = serverData[i].trim();
            if (urlSource.startsWith(server)) {
                currentServer = server
                continue;
            }
            switchServer = server;
        }

        for (let j = 0; j < images.length; j++) {
            let urlReplace = $(images[j]).attr("src");
            let afterUrl = !urlReplace.startsWith("http") ?   switchServer + urlReplace : urlReplace.replace(currentServer, switchServer);
            $(images[j]).closest(ViewComic.clazzImageBlock).html(ViewComic.buildImage(afterUrl, j));
        }
        notification.show(messages.reloadedServer.replace("{0}", images.length), notification.Info);
    },
    doSearchChapter: () => {
        var value = $(ViewComic.inputSearchClass).val();
        if (value === "") {
            $(ViewComic.btnChapterClass).removeClass("hide");
        } else {
            let chaptersFilter = $(ViewComic.btnChapterClass);
            for (index in chaptersFilter) {
                indexC = parseInt(index);
                let name = $(chaptersFilter[indexC]).text();
                if (name.includes(value)) {
                    $(chaptersFilter[indexC]).removeClass("hide");
                } else {
                    $(chaptersFilter[indexC]).addClass("hide");
                }
            }
        }
    },
    toggleSetting: () => {
        if ($(ViewComic.settingClass).hasClass(ViewComic.active)) {
            $(ViewComic.settingClass).removeClass(ViewComic.active);
            setTimeout(() => $(ViewComic.switchServerClass).addClass(ViewComic.active), 500);
            return;
        }
        $(ViewComic.switchServerClass).removeClass(ViewComic.active)
        $(ViewComic.settingClass).addClass(ViewComic.active);
    },
    goNextPrevChapter: (_self) => {
        var url = $(_self).attr("data-href");
        if (url) {
            doRedirect(url);
        }
    },
    doFollow: (_self) => {
        if (!commonJS.isAuthenticated()) {
            notification.show(commonMessages.requiredLogin, notification.Info);
            return;
        }
        $.ajax({
            url: jsContext + `/api/comic/follow`,
            beforeSend: beforeAuth,
            type: 'POST',
            dataType: "json",
            data: {comicId: comic.id, push: false, followSessionId: commonJS.getLocalSession()}
        }).done(res => {
            if (res.status === true) {
                if (res.result.wait) {
                    notification.show(res.result.message, notification.Warn);
                } else {
                    notification.show(res.result.message, notification.Success);
                }
            } else {
                notification.show(res.messages[0], notification.Warn);
            }
        }).catch(e => {
            notification.show(res.messages[0], notification.Warn);
        })
    },
    doReport: () => {
        if ($(ViewComic.btnSendReportClass).hasClass("btn-disable")) {
            return;
        }

        const val = $(ViewComic.txtContentReport).val();
        if (!val) {
            notification.show(messages.reasonRequired, notification.Warn);
            return;
        }
        const confirm = window.confirm(messages.confirmSubmitReport);
        if (confirm) {
            $(ViewComic.btnSendReportClass).addClass("btn-disable");
            $.ajax({
                url: jsContext + `/api/chapter/report`,
                type: 'POST',
                beforeSend: beforeAuth,
                dataType: "json",
                data: {
                    comicId: comic.id,
                    comicName: comic.comicName,
                    chapterId: chapterInfo.id,
                    chapterNumber: comic.currentNumber,
                    reason: val,
                    localSessionId: commonJS.getLocalSession()
                }})
                .done(res => {
                    if (res.status === true) {
                        diaLogPopup.closeAndOpenDiaLog(ViewComic.dialogReportId);
                        $(ViewComic.txtContentReport).val("");
                        notification.show(messages.reportSuccess, notification.Success);
                        commonJS.storeLocalSession(res.result.extraData)
                    } else {
                        notification.showNotification(res.messages[0], notification.Error);
                    }
                    $(ViewComic.btnSendReportClass).removeClass("btn-disable");
                })
        }
    },
    saveReadComic: () => {
        try {
            let readChapter = JSON.parse(commonJS.getLocal("read_chapter"));
            if (!readChapter) {
                readChapter = {};
            }
            let oldList = readChapter[comic.nameEn];
            commonJS.initReadChapter(comic.nameEn);
            if (!oldList) {
                oldList = chapterInfo.numberChapter;
            } else {
                if (oldList.includes(chapterInfo.numberChapter + ",")) {
                    return;
                }
                oldList = chapterInfo.numberChapter + "," + oldList;
            }
            readChapter[comic.nameEn] = oldList;

            commonJS.saveLocal("read_chapter", JSON.stringify(readChapter));
        } catch (e) {}
    },
    saveHistoryComic: () => {
        try {
            let data = JSON.parse(commonJS.getHistoryData());

            if (!data) {
                data = [];
            }

            // Find and assign new chapter
            var chapter = comic.currentNumber;
            var date = "";
            try {
                date = new Date().toLocaleString();
            } catch (ex) {}
            const index = data.map((e) => e.nameEn).indexOf(comic.nameEn);
            if (index !== -1) {
                let c = data[index];
                if (!!chapter) {
                    c = {
                        nameEn: comic.nameEn,
                        name: comic.comicName,
                        otherName: comic.otherName,
                        photo: comic.photo,
                        chapter: chapter,
                        date: date
                    };
                }
                data.splice(index, 1);
                data.unshift(c);
            } else {
                data.unshift({
                    nameEn: comic.nameEn,
                    name: comic.comicName,
                    otherName: comic.otherName,
                    photo: comic.photo,
                    chapter: chapter,
                    date: date
                });
            }
            // Max 300 comics
            if (data.length > 300) {
                data.pop();
            }
            let trackingChapter = JSON.parse(commonJS.getTrackingData());
            if (!trackingChapter) {
                trackingChapter = {};
            }
            trackingChapter[comic.nameEn] = chapterInfo.numberChapter;
            const obj = Object.keys(trackingChapter);
            if (obj.length > 300) {
                const key = obj[0];
                delete trackingChapter[key];
            }

            commonJS.saveLocal("tracking-chapter", JSON.stringify(trackingChapter));
            commonJS.saveLocal("history_info", JSON.stringify(data));
        } catch (e) {}
    },
    pressingChangeChapter: () => {
        if (isWindowUser === "true") {
            setTimeout(() => {
                function pressKeyChangeChapter(e) {
                    try {
                        const isFocusCMT = $(ViewComic.inputComment).is(":focus") || $(ViewComic.txtContentReport).is(":focus");
                        if (isFocusCMT) {
                            return;
                        }
                        if (e.keyCode === 37) {
                            const leftArrow = document.querySelector(".nav-prev");
                            if (!!leftArrow) {
                                leftArrow.click();
                            }
                        } else if (e.keyCode === 39) {
                            const rightArrow = document.querySelector(".nav-next");
                            if (!!rightArrow) {
                                rightArrow.click();
                            }
                        }
                    } catch (e) {}
                }

                document.onkeydown = pressKeyChangeChapter;
            }, 2000);
        }
    },
    trackUser: (width, append = "", direct = false) => {
        $.ajax({
            url: jsContext + `/api/user/tracking`,
            beforeSend: beforeAuth,
            type: 'POST',
            dataType: "json",
            data: {
                width: width,
                name: !!localStorage.getItem("user_profile") + "|" + comic.nameEn + append,
                number: comic.currentNumber,
                direct: direct
            }
        }).done(res => {
            if (res.status && res.result) {
                localStorage.setItem("user_profile", "Empty");
            }
        })
    },
    buildImage:(url, index) => {
        return `<div class="lds-loading"><div></div><div></div><div></div></div><img class="image" data-retry="1" onerror="retryImage(this);" onload="finishLoad(this);" src="` + url + `" alt="`+ index +`.jpg_đang thử tải lại..." oncontextmenu="return false"/>`
    }
}
// Search later
$(document).ready(function () {
    ViewComic.saveHistoryComic();
    ViewComic.trackUser(window.innerWidth);
    ViewComic.pressingChangeChapter();
    ViewComic.saveReadComic();
    $(document).on("contextmenu", '*', function (e) {
        e.preventDefault();
        return false;
    });
    $(document).on("click", ViewComic.toggleSettingClass, function () {
        ViewComic.toggleSetting();
    });
    $(document).on("click", ViewComic.navListClass + "," + ViewComic.btnCloseClass, function () {
        diaLogPopup.closeAndOpenDiaLog(ViewComic.dialogId, true);
    });
    $(document).on("click", ViewComic.navReportClass + "," + ViewComic.btnCloseReport + "," + ViewComic.btnCancelClass, function () {
        diaLogPopup.closeAndOpenDiaLog(ViewComic.dialogReportId);
        $(ViewComic.txtContentReport).focus();
    });
    $(document).on("click", ViewComic.navNextClass + "," + ViewComic.navPrevClass + "," + ViewComic.btnHomeSettingClass, function () {
        ViewComic.goNextPrevChapter(this);
    });
    $(document).on("click", ViewComic.btnSendReportClass, function () {
        ViewComic.doReport();
    });
    $(document).on("click", ViewComic.btnFollowClass, function () {
        ViewComic.doFollow();
    });
    $(document).on("click", ViewComic.switchServerClass, function (e) {
        e.preventDefault();
        $(ViewComic.switchServerClass).addClass("disabled")
        ViewComic.doSwitchingServer();
    });
    $(document).on("keyup", ViewComic.inputSearchClass, function () {
        ViewComic.doSearchChapter();
    });
    $(document).on("mousedown", ViewComic.clazzImageArea, function (e) {
        return;
        if (isIOSUser === 'true') {
            return;
        }
        try {
            if (e.which === 3) {
                ViewComic.doClickPanelOnImage(e);
            }
        } catch (ex) {
            if ($(ViewComic.clazzFixedClickPanel).hasClass(ViewComic.active)) {
                $(ViewComic.clazzFixedClickPanel).removeClass(ViewComic.active);
            }
        }
    });

    // $(document).on("touchstart", ViewComic.clazzImageArea, function (e) {
    //     if (e.cancelable) {
    //         e.preventDefault();
    //     }
    //     e.stopPropagation();
    //     if ($(ViewComic.clazzFixedClickPanel).hasClass(ViewComic.active)) {
    //         $(ViewComic.clazzFixedClickPanel).removeClass(ViewComic.active);
    //     }
    //     ViewComic.keepTouch = setTimeout(() => ViewComic.doClickPanelOnImage(e, true), 2000);
    // });
    //
    // $(document).on("touchend", ViewComic.clazzImageArea, function (e) {
    //     if (e.cancelable) {
    //         e.preventDefault();
    //     }
    //     e.stopPropagation();
    //     if (ViewComic.keepTouch !== undefined) {
    //         clearTimeout(ViewComic.keepTouch);
    //     }
    // });
    // $(document).on("touchmove", ViewComic.clazzImageArea, function (e) {
    //     if (e.cancelable) {
    //         e.preventDefault();
    //     }
    //     e.stopPropagation();
    //     if (ViewComic.keepTouch !== undefined) {
    //         clearTimeout(ViewComic.keepTouch);
    //         if ($(ViewComic.clazzFixedClickPanel).hasClass(ViewComic.active)) {
    //             $(ViewComic.clazzFixedClickPanel).removeClass(ViewComic.active);
    //         }
    //     }
    // });
});


try {
    var doc = document.documentElement;
    var w = window;
    var prevScroll = w.scrollY || doc.scrollTop;
    var curScroll;
    var direction = 0;
    var prevDirection = 0;
    var checkScroll = function() {
        curScroll = w.scrollY || doc.scrollTop;
        if (135 > curScroll) {
            $('.top-move-pannel').removeClass('fixed').removeClass("fixed-toggle");
            $('.temp-bar').removeClass('fixed')
            return;
        } else {
            $('.top-move-pannel').addClass('fixed');
            $('.temp-bar').addClass('fixed')
        }
        if (curScroll > prevScroll && curScroll - prevScroll > 15) {
            direction = 2;
        }
        else if (curScroll < prevScroll) {
            direction = 1;
        }
        if (doc.scrollTop + 2000 > document.querySelector(".main").offsetHeight) {
            direction = 1;
        }
        if (direction !== prevDirection) {
            toggleHeader(direction, curScroll);
        }
        prevScroll = curScroll;
    };
    var toggleHeader = function(direction, curScroll) {
        if (direction === 2 && curScroll > 20) {
            $('.top-move-pannel').removeClass('fixed-toggle');
            prevDirection = direction;
            return;
        }
        if (direction === 1) {
            $('.top-move-pannel').addClass('fixed-toggle');
            prevDirection = direction;
            return;
        }
    };
    window.addEventListener('scroll', checkScroll);
} catch (e) {
}

function finishLoad(self, addClass = "finished") {
    $(self).addClass(addClass).parent().children(".lds-loading").hide();
}

function retryImage(self) {
    let retryCount = parseInt($(self).attr("data-retry"));
    if (retryCount > 0) {
        $(self).attr("alt", "Tải thất bại");
        finishLoad(self, "");
    } else {
        setTimeout(() => {
            $(self).attr("data-retry", retryCount + 1);
            const src = $(self).attr("src");
            $(self).attr("src", null);
            $(self).attr("src", src);
        }, 3000);
    }
}