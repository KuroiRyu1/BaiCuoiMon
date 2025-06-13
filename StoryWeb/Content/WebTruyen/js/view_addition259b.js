const ViewValidate = {
    idMode: "#mode-view",
    clazzImageSection: ".main-images .image-section",
    clazzWarning: ".warning-level",
    clazzValidateDialog: "#dialog-view-validate",
    btnLogin: ".btn-view-login",
    btnLogout: ".btn-view-logout",
    btnRequest: ".btn-view-up",
    clazzContent: "#dialog-view-validate .content",
    clazzBottomButton: ".view-bottom-panel",
    lengthImage: 15,
    appendImages: [],
    appendAllImages: "",
    positionScroll: 0,
    bufferLoaded: 4000 + window.innerHeight,
    loaded: false,
    initState: (comicId, chapterNumber) => {
        const mode = $(ViewValidate.idMode).val();
        if (!commonJS.isAuthenticated() && mode !== "normal") {
            setTimeout(() => {
                $(ViewValidate.clazzContent).html(messages.requiredAuth);
                diaLogPopup.closeAndOpenDiaLog(ViewValidate.clazzValidateDialog);
                $(ViewValidate.btnLogin).show();
                $(ViewValidate.clazzWarning).remove();
            }, 500)
            return;
        }

        setTimeout(() => {
            if (mode === "normal") {
                ViewValidate.doLoadNormalChapter();
                return;
            }
            if (mode === "limit") {
                ViewValidate.doLoadLimitation();
                return;
            }
            if (mode === "auth") {
                ViewValidate.doLoadAuthChapter();
                return;
            }
        }, 300);
    },
    processingImage: (res) => {
        const data = res.result.data;
        const length = data.length;
        for (let i = 0; i < length; i++) {
            if (i < ViewValidate.lengthImage) {
                $(ViewValidate.clazzImageSection).append(ViewValidate.buildImage(data[i], i));
            } else {
                let modIndex = Math.floor(i / ViewValidate.lengthImage) - 1;
                let appendData = ViewValidate.appendImages[modIndex];
                if (!appendData) {
                    appendData = ""
                }
                appendData += ViewValidate.buildImage(data[i], i);
                ViewValidate.appendImages[modIndex] = appendData;
                ViewValidate.appendAllImages += ViewValidate.buildImage(data[i], i);
            }
        }
        if (ViewValidate.appendImages.length > 0) {
            ViewValidate.checkScrollImages();
        }
    },
    processingError: (res) => {
        if (res.result.block) {
            ViewValidate.initUser();
            notification.show(res.result.message, notification.Error)
            setTimeout(() => doRedirect("/"), 5000);
        } else {
            $(ViewValidate.btnRequest).show();
            $(ViewValidate.clazzContent).html(messages.requiredLimit);
            diaLogPopup.closeAndOpenDiaLog(ViewValidate.clazzValidateDialog);
        }
    },
    doLoadNormalChapter: () => {
        $(ViewValidate.clazzWarning).remove();
        let res = JSON.parse(chapterInfo.chapterJson);
        ViewValidate.processingImage(res.body);
    },
    doLoadAuthChapter: () => {
        $.ajax({
            url: jsContext + `/api/chapter/auth`,
            type: 'POST',
            beforeSend: beforeAuth,
            dataType: "json",
            data: { comicId: comic.id, chapterNumber: comic.currentNumber, nameEn: comic.nameEn}
        }).done(res => {
            $(ViewValidate.clazzWarning).remove();
            if (res.result.state) {
                ViewValidate.processingImage(res);
            } else {
                ViewValidate.processingError(res);
            }
        })
    },
    doLoadLimitation: () => {
        $.ajax({
            url: jsContext + `/api/chapter/limitation`,
            type: 'POST',
            beforeSend: beforeAuth,
            dataType: "json",
            data: { comicId: comic.id, chapterNumber: comic.currentNumber, nameEn: comic.nameEn}
        }).done(res => {
            $(ViewValidate.clazzWarning).remove();
            if (res.result.state) {
                ViewValidate.processingImage(res);
            } else {
                ViewValidate.processingError(res);
            }
        })
    },
    initUser: (isBlock = true) => {
        $.ajax({
            url: jsContext + "/api/cleanSession",
            type: 'POST'
        }).done(res => {
            if (res.status && res.result) {
                localStorage.clear();
                if (isBlock) {
                    localStorage.setItem("user_profile", "Empty");
                }
            }
        });
    },
    buildImage:(url, index) => {
        return `<div class="img-block"><div class="lds-loading"><div></div><div></div><div></div></div><img class="image" data-retry="0" onerror="retryImage(this);" onload="finishLoad(this);" src="` + url + `" alt="`+ index +`.jpg_đang thử tải lại..." oncontextmenu="return false"/></div>`
    },
    loadRemainImages: (isAll = false) => {
      if (isAll) {
          $(ViewValidate.clazzImageSection).append(ViewValidate.appendAllImages);
          return;
      }

      if (ViewValidate.appendImages.length > 0) {
          $(ViewValidate.clazzImageSection).append(ViewValidate.appendImages.shift());
          setTimeout(() => ViewValidate.loaded = false, 2000);
      }
    },
    verifyScroll: () => {
        ViewValidate.positionScroll = window.scrollY || document.documentElement.scrollTop;
        if (ViewValidate.positionScroll + ViewValidate.bufferLoaded > $(ViewValidate.clazzBottomButton).offset().top) {
            ViewValidate.loadRemainImages();
            ViewValidate.loaded = true;
        }
    },
    checkScrollImages: () => {
        try {
            setTimeout(() => {
                ViewValidate.verifyScroll();
                window.addEventListener('scroll', () => {
                    if (ViewValidate.loaded) {
                        return;
                    }
                    try {
                        ViewValidate.verifyScroll();
                    } catch (e) {
                        ViewValidate.loadRemainImages(true);
                        ViewValidate.loaded = true;
                    }
                });
            }, 1000);
        } catch (e) {
            ViewValidate.loadRemainImages(true);
            ViewValidate.loaded = true;
        }
    }
}
$(document).ready(function () {
    ViewValidate.initState();
})