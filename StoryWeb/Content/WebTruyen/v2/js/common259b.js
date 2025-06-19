window.isMobile = window.innerWidth <= 750;
const beforeAuth = (request) => {
    const token = localStorage.getItem("Authorization");
    if (!!token) {
        request.setRequestHeader("Authorization", token);
    }
};
const clearTokenUser = () => {
    localStorage.removeItem("Authorization");
    localStorage.removeItem("user_info");
    localStorage.removeItem("localSession");
    localStorage.removeItem("tracking-chapter");
    localStorage.removeItem("history_info");
};
const getUserData = () => {
    const jsonUser = localStorage.getItem("user_info");
    return !!jsonUser ? JSON.parse(jsonUser) : {};
};

const doRedirect = (url) => {
    window.location.href = url;
};

const registerPushNotification = (ajaxCallBack, additionCallBack = undefined, deniedCallback = undefined, errorCallback = undefined) => {
    if (!window.Notification) {
        console.log("Not support");
    } else {
        Notification.requestPermission().then(status => {
            //Granted
            if (status === "granted") {
                if ('serviceWorker' in navigator && 'PushManager' in window) {
                    messaging.requestPermission().then(() => messaging.getToken({vapidKey: "BHl6SGKtTUmywlxRJFywQviemtgHCsV5kt-6yiXURsQOhD5m_MJWJg4EW6-75i2kMg9v8anyxbHcLv06o-msSuw"}))
                    .then(function (token) {
                        ajaxCallBack(token).done(res => {
                            // Todo nothing
                            if (additionCallBack !== undefined) {
                                additionCallBack(status, res);
                            }
                        })
                    });
                }
            }
            if (status === "denied") {
                deniedCallback(status);
            }
        }).catch(e => {
            if (errorCallback !== undefined) {
                errorCallback(e, status);
            }
        })
    }
};

function mergeArrays(array1, array2) {
    return [...array1, ...array2].reduce((acc, current) => {
        const x = acc.find(item => item.nameEn === current.nameEn);
        if (!x) {
            acc.push(current);
        }
        return acc;
    }, []);
}

function mergeData(data1, data2) {
    var result = {};

    // Helper function to merge two comma-separated strings into one
    function mergeValues(value1, value2) {
        // Convert values to arrays
        var array1 = value1 ? value1.split(',') : [];
        var array2 = value2 ? value2.split(',') : [];

        // Combine arrays and remove duplicates
        var combined = [];
        var seen = {};

        function addValues(array) {
            for (var i = 0; i < array.length; i++) {
                var item = array[i];
                if (!seen[item]) {
                    seen[item] = true;
                    combined.push(item);
                }
            }
        }

        addValues(array1);
        addValues(array2);

        // Join combined array into a comma-separated string
        return combined.join(',');
    }

    // Get all unique keys from both data objects
    var allKeys = {};
    for (var key in data1) {
        if (data1.hasOwnProperty(key)) {
            allKeys[key] = true;
        }
    }
    for (var key in data2) {
        if (data2.hasOwnProperty(key)) {
            allKeys[key] = true;
        }
    }

    // Merge data for each key
    for (var key in allKeys) {
        if (allKeys.hasOwnProperty(key)) {
            var value1 = data1[key];
            var value2 = data2[key];

            // Merge the values for the current key
            result[key] = mergeValues(value1, value2);
        }
    }

    return result;
}

function mergeDataTracking(data1, data2) {
    for (var key in data2) {
        if (!data1.hasOwnProperty(key)) {
            data1[key] = data2[key];
        }
    }

    return data1;
}


const formattedNumber = new Intl.NumberFormat('en-US');

const tracking = {
    get: () => {
        return localStorage.getItem("tracking-chapter");
    },
    getNovel: () => {
        return localStorage.getItem("tracking-novel-chapter");
    },
    getLocalSession: () => {
        return localStorage.getItem("localSession");
    }
}

const diaLogPopup = {
    dialogActive: "dialog-active",
    disableOverflow: "over-flow-y-disabled",
    diaLogBodyClass: ".dialog-body .active",
    htmlTag: "html",
    closeAndOpenDiaLog: (_idSelector, isScroll = false) => {
        if ($(_idSelector).hasClass(diaLogPopup.dialogActive)) {
            $(_idSelector).removeClass(diaLogPopup.dialogActive);
            $(diaLogPopup.htmlTag).removeClass(diaLogPopup.disableOverflow);
            return;
        }
        $(_idSelector).addClass(diaLogPopup.dialogActive);
        $(diaLogPopup.htmlTag).addClass(diaLogPopup.disableOverflow);
        if (isScroll) {
            document.querySelector(_idSelector + " " + diaLogPopup.diaLogBodyClass).scrollIntoView({block: "center"});
        }
    }
}

const commonJS = {
    scrollTopClass: ".scroll-top",
    scrollTopAllClass: ".all-scroll-top",
    urlFacebook: "/api/login/facebook",
    urlGoogle: "/api/login/google",
    scrollTop: () => {
        $("html, body").animate({scrollTop: 0}, "slow");
        return false;
    },
    saveLocal: (key, value) => {
        localStorage.setItem(key, value);
    },
    removeLocal: (key) => {
        localStorage.removeItem(key);
    },
    hasLocal: (key) => {
        return !!localStorage.getItem(key);
    },
    getLocal: (key) => {
        return localStorage.getItem(key);
    },
    storeLocalSession: (session) => {
        commonJS.saveLocal("localSession", session);
    },
    getLocalSession: () => {
        return commonJS.getLocal("localSession");
    },
    getHistoryData: () => {
        return commonJS.getLocal("history_info");
    },
    getHistoryNovelData: () => {
        return commonJS.getLocal("history_novel_info");
    },
    getTrackingData: () => {
        return commonJS.getLocal("tracking-chapter");
    },
    getTrackingNovelData: () => {
        return commonJS.getLocal("tracking-novel-chapter");
    },
    isAuthenticated: () => {
        const user = getUserData();
        if (!user) {
            return false;
        }
        return !(!user.token || !user.name || !user.type || !user.image);
    },
    redirectAfterLogin: (url) => {
        var return_path = window.location.href;
        commonJS.saveLocal("return_path", return_path);
        window.location.href = url;
    },
    call: (type, url) => {
        $.ajax({
            url: jsContext + url,
            type: 'POST',
            dataType: "json",
            data: {type}
        }).done(res => {
            if (res.status === true) {
                commonJS.redirectAfterLogin(res.result);
            } else {
                notification.show(res.messages[0], notification.Error);
            }
        });
    },
    doFacebookSignIn: () => commonJS.call("FB", commonJS.urlFacebook),
    doGoogleSignIn: () => commonJS.call("GA", commonJS.urlGoogle),
    checkReportFixed: () => {
        $.ajax({
            url: jsContext + `/api/chapter/reportFixed`,
            beforeSend: beforeAuth,
            type: 'POST',
            dataType: "json",
            data: {localSessionId : commonJS.getLocalSession()}
        }).done(res => {
            if (res.status && res.result.state) {
                notification.show(res.result.msg, notification.Info);
                commonJS.storeLocalSession(res.result.extraData);
            }
        });
    },
    checkBannedUser: () => {
        if (commonJS.isAuthenticated() && !!localStorage.getItem("user_profile")) {
            $.ajax({
                url: jsContext + `/api/user/checked`,
                beforeSend: beforeAuth,
                type: 'POST',
                dataType: "json"
            }).done(res => {
                if (res !== null && res.status && res.result.status) {
                    localStorage.removeItem("user_profile");
                }
            })
        }
    },
    initReadChapter: (comicEn) => {
        try {
            let readChapter = JSON.parse(commonJS.getLocal("read_chapter"));
            if (!readChapter) {
                return;
            }

            let oldList = readChapter[comicEn];
            if (!oldList) {
                return;
            }

            oldListReplaced = "." + oldList
                .replaceAll(".", "\\.")
                .replaceAll(",", ",.");

            $(oldListReplaced).addClass("read");
        } catch (e) {}
    },
    initAllReadComic: () => {
        try {
            if (window.Worker) {
                let readChapter = JSON.parse(commonJS.getLocal("read_chapter"));
                if (!readChapter) {
                    return;
                }
                const elements = document.querySelectorAll('.card-reader[data-card]');
                const dataCardValues = Array.from(elements).map(el => el.getAttribute('data-card'));

                const data = {
                    comics: dataCardValues,
                    chapters: readChapter
                };
                const worker = new Worker(jsContext + "/contents/v2/js/worker.js");
                worker.postMessage(data);
                worker.onmessage = function (e) {
                    const result = e.data;
                    if (result) {
                        $(result).addClass("sticker-read");
                    }
                };

                worker.onerror = function (error) {
                    console.error('Error in worker:', error.message);
                };

            } else {
                let readChapter = JSON.parse(commonJS.getLocal("read_chapter"));
                if (!readChapter) {
                    return;
                }

                const result = Object.entries(readChapter)
                    .flatMap(([key, value]) => value.split(',')
                        .map(val => `.${key}-${val.trim()}`)
                    ).join(', ');

                $(result).addClass("sticker-read");
            }
        } catch (e) {}
    }
}

const notification = {
    Container: ".notification-container",
    MainClass: ".notification",
    ServerContainer: ".notification-container-server",
    Server: ".notification-server",
    Leave: "notification-leave notification-leave-active",
    Enter: "notification-enter notification-enter-active",
    Success: "notification-show",
    Error: "notification-error",
    Info: "notification-show",
    Warn: "notification-show",
    Message: ".notification-message > .message",
    addLoading: () => {
        return `<div class="lds-loading"><div></div><div></div><div></div></div>`;
    },
    add: (message, clazz) => {
        return '<div class="notification notification-enter ' + clazz + '"> <div class="notification-message" role="alert"> <div class="message">' + message + '</div> </div><div class="timer"></div></div>'
    },
    addServer: (data) => {
        return '<div class="notification-server notification-enter"><a class="notification-message-server" href="' + data.url + '" target="_blank"  rel="noopener noreferrer" onClick="notification.hidePush(this)" role="alert"><div class="image">' +
            '<img src="' + data.image + '" alt="' + data.image + '"></div><div class="push-content"><div class="push-title">' + data.title + '</div><div class="push-chapter">' + commonMessages.pushNewChapter + ' ' + data.chapter + '</div>' +
            '<div class="push-time">' + commonMessages.pushComicItem + data.time + '</div><div class="push-author">' + commonMessages.pushAuthor + ' ' + data.author + '<span onClick="notification.closePush(event)">X<span></div></div></a><div class="timer"></div></div>';
    },
    show: (msg, clazz, timeout = 3000) => {
        if (notification.Error === clazz) {
            timeout = 5000;
        }

        $(notification.Container).children("div").append(notification.add(msg, clazz));
        const position = $(notification.MainClass).length;
        if (position > 0) {
            setTimeout(() => $($(notification.MainClass)[position - 1]).addClass("notification-enter-active").css("--active-timer", timeout + "ms"), 100);
        }
        setTimeout(() => notification.hide(), timeout);
    },
    hide: () => {
        if ($(notification.MainClass).length > 0) {
            $($(notification.MainClass)[0]).addClass(notification.Leave);
            setTimeout(() => $($(notification.MainClass)[0]).remove(), 500);
        }
    },
    showPush: (data, timeout = 60000) => {
        $(notification.ServerContainer).children("div.push").append(notification.addServer(data));
        let position = $(notification.Server).length;
        if (position > 0) {
            setTimeout(() => $($(notification.Server)[position - 1]).addClass("notification-enter-active").css("--active-timer", timeout + "ms"), 100);
        }
        setTimeout(() => notification.hidePushTimeout(), timeout);
    },
    hidePushTimeout: () => {
        const position = $(notification.Server).length;
        if (position > 0) {
            $($(notification.Server)[0]).addClass(notification.Leave);
            setTimeout(() => $($(notification.Server)[0]).remove(), 300);
        }
    },
    hidePush: (_self) => {
        $(_self).parent().addClass(notification.Leave);
        setTimeout(() => $(_self).parent().remove(), 300);
    },
    closePush: (event) => {
        event.preventDefault();
    },
}

const UserAction = {
    doLoadReminder: (isShowDiaLog = true) => {
        if (isShowDiaLog) {
            diaLogPopup.closeAndOpenDiaLog(headers.idDialogReminder);
        }
        if ($(headers.idDialogReminder).hasClass(headers.inProgress)) {
            return;
        }

        setTimeout(() => {
            $.ajax({
                url: jsContext + `/api/user/getAllReminders`,
                beforeSend: beforeAuth,
                type: 'GET'
            }).done(res => {
                if (res.status) {
                    const data = res.result;
                    $(headers.idDialogReminder).addClass(headers.inProgress);
                    if (data.length === 0) {
                        $(headers.dialogBodyReminder).html(`<div class="pa-2">` + commonMessages.noNotification + `</div>`)
                        return;
                    }
                    $(headers.dialogBodyReminder).html("");
                    for (let i = 0 ; i < data.length; i++) {
                        if (data[i].type === 'MESSAGE_USER') {
                            $(headers.dialogBodyReminder).append(headers.drawReminderHtml(data[i], true));
                        } else {
                            $(headers.dialogBodyReminder).append(headers.drawReminderHtml(data[i]));
                        }
                    }
                }
            });
        }, 500);
    },
    doSignOut: () => {
        const confirm = window.confirm(commonMessages.confirmSignOut);
        if (confirm) {
            $.ajax({
                url: jsContext + "/api/cleanSession",
                type: 'POST'
            }).done(res => {
                if (res.status && res.result) {
                    // Clear store user
                    const user = getUserData();
                    if (user.type === "FB" && !!window.FB) {
                        try {
                            window.FB.logout();
                        } catch (e) {
                            window.FB.logout;
                        }
                    }
                    clearTokenUser();
                    window.location.reload();
                }
            });
        }
    },
    checkPermissionUpdateName: () => {
        $.ajax({
            url: jsContext + `/api/user/checkChangeName`,
            beforeSend: beforeAuth,
            type: 'GET'
        }).done(res => {
            if (res.status) {
                const data = res.result;
                $(headers.dialogInputNameRename).val(data.name);
                $(headers.dialogInputTitleRename).val(data.title);
                diaLogPopup.closeAndOpenDiaLog(headers.idDialogRename);
                diaLogPopup.closeAndOpenDiaLog(headers.idDialogUser);
            } else {
                notification.show(res.messages[0], notification.Warn);
            }
        });
    },
    updateUserInformation: (_self) => {
        if ($(_self).hasClass(headers.inProgress)) {
            return;
        }
        const isAgree = confirm(commonMessages.confirmChangeInfo);
        if (isAgree) {
            $(_self).addClass(headers.inProgress);
            const name = $(headers.dialogInputNameRename).val();
            const title = $(headers.dialogInputTitleRename).val();
            if ((name.includes("<") && name.includes(">")) || (title.includes("<") && title.includes(">"))) {
                notification.show("Nội dung có chứa ký tự đặc biệt", notification.Error);
                return;
            }
            $.ajax({
                url: jsContext + `/api/user/updateName`,
                beforeSend: beforeAuth,
                type: 'POST',
                data: {name: name, title: title}
            }).done(res => {
                if (res.status) {
                    const data = res.result;
                    const user = getUserData();
                    user.rank = data.rank;
                    user.name = data.name;
                    commonJS.saveLocal("user_info", JSON.stringify(user));
                    notification.show(data.message, notification.Success);
                    diaLogPopup.closeAndOpenDiaLog(headers.idDialogRename)
                } else {
                    modal.showNotification(res.messages[0], notification.Warn);
                }
                $(_self).removeClass(headers.inProgress);
            });
        }
    },
    requestUpgrade: (_self) => {
        if ($(_self).hasClass(headers.inProgress)) {
            return;
        }
        const isAgree = confirm(commonMessages.confirmUpgrade);
        if (isAgree) {
            $(_self).addClass(headers.inProgress);
            $.ajax({
                url: jsContext + `/api/user/upgrade`,
                beforeSend: beforeAuth,
                type: 'POST',
                data: {session: commonJS.getLocalSession(), history: commonJS.getHistoryData()}
            }).done(res => {
                if (res.status) {
                    notification.show(commonMessages.userSendRequest, notification.Warn);
                } else {
                    notification.show(res.messages[0], notification.Warn);
                }
                $(_self).removeClass(headers.inProgress);
            });
        }
    },
    doRestoreDeviceData: (_self) => {
        if ($(_self).hasClass(headers.inProgress)) {
            return;
        }
        $(_self).addClass(headers.inProgress);

        $.ajax({
            url: jsContext + `/api/user/restoreData`,
            beforeSend: beforeAuth,
            type: 'POST',
            dataType: "json"
        }).done(res => {
            if (res.status === true) {
                const data = res.result;
                const isAgree = confirm(data.message);
                if (isAgree) {
                    commonJS.storeLocalSession(data.localSessionId);
                    commonJS.saveLocal("history_info", data.historyData);
                    commonJS.saveLocal("tracking-chapter", data.trackingData);
                    notification.show(commonMessages.deviceSuccess, notification.Success);
                }
            } else {
                notification.show(res.messages[0], notification.Info);
            }
            $(_self).removeClass(headers.inProgress);
        });
    },
    doSkipAds: (_self) => {
        if ($(_self).hasClass(headers.inProgress)) {
            return;
        }

        $(_self).addClass(headers.inProgress);

        $.ajax({
            url: jsContext + `/api/user/performSkipAds`,
            beforeSend: beforeAuth,
            type: 'POST',
            dataType: "json"
        }).done(res => {
            if (res.status) {
                if (res.result.state) {
                    notification.show(res.result.msg, notification.Info);
                    setTimeout(() => window.location.reload(), 3000);
                } else {
                    notification.show(res.result.msg, notification.Error);
                }
            } else {
                notification.show(res.data.messages[0], notification.Error);
            }
            $(_self).removeClass(headers.inProgress);
        });
    },
    changeDesign: (_self) => {
        if ($(_self).hasClass(headers.inProgress)) {
            return;
        }

        const isAgree = confirm(commonMessages.changeDesignMsg);
        if (!isAgree) {
            return;
        }

        $(_self).addClass(headers.inProgress);
        $.ajax({
            url: jsContext + `/api/user/changeDesign`,
            beforeSend: beforeAuth,
            type: 'POST',
            dataType: "json"
        }).done(res => {
            if (res.status && res.result.state) {
               setTimeout(() => window.location.reload(), 1000);
            } else {
                notification.show(res.data.messages[0], notification.Error);
            }
            $(_self).removeClass(headers.inProgress);
        });
    },
    doActiveAuto: () => {
        let msg = commonMessages.activeAuto;
        let isTrigger = commonJS.getLocal("trigger_auto");
        if (isTrigger) {
            msg = commonMessages.inactiveAuto;
        }

        const isAgree = confirm(msg);
        if (!isAgree) {
            return;
        }

        if (!isTrigger) {
            notification.show(commonMessages.activeAutoMessage, notification.Info);
            UserAction.doAutoFetch();
        } else {
            localStorage.removeItem("trigger_auto");
            notification.show(commonMessages.inactiveAutoMessage, notification.Info);
        }
    },
    initAutoSync: () => {
        try {
            let isTrigger = commonJS.getLocal("trigger_auto");
            if (!isTrigger) {
                return;
            }
            if (new Date().getTime() > isTrigger) {
                UserAction.doAutoFetch();
            }
        } catch (e) {}
    },
    doAutoFetch: () => {
        try {
            $.ajax({
                url: jsContext + `/api/user/auto-fetch`,
                beforeSend: beforeAuth,
                type: 'POST',
                dataType: "json"
            }).done(res => {
                if (res.status) {
                    let currReadChapter = {};
                    let currTrackingChapter = {};
                    let currHistoryData = [];

                    if (res.result.hasData) {
                        currReadChapter = JSON.parse(res.result.readDataTracking);
                        currTrackingChapter = JSON.parse(res.result.trackingData);
                        if (res.result.historyData !== "{}") {
                            currHistoryData = JSON.parse(res.result.historyData);
                        }
                    }

                    let trackingChapter = JSON.parse(commonJS.getTrackingData());
                    let readChapter = JSON.parse(commonJS.getLocal("read_chapter"));
                    let historyData = JSON.parse(commonJS.getHistoryData());

                    if (!readChapter) {
                        readChapter = {};
                    }

                    if (!trackingChapter) {
                        trackingChapter = {};
                    }

                    if (!historyData) {
                        historyData = [];
                    }


                    let resultReadChapter = mergeData(currReadChapter, readChapter);
                    let resultTrackingChapter = mergeDataTracking(trackingChapter, currTrackingChapter);
                    let resultHistoryData = mergeArrays(historyData, currHistoryData);
                    let jsonRead = JSON.stringify(resultReadChapter);
                    let jsonTracking = JSON.stringify(resultTrackingChapter);
                    let jsonHistory = JSON.stringify(resultHistoryData);
                    commonJS.saveLocal("read_chapter", jsonRead);
                    commonJS.saveLocal("tracking-chapter", jsonTracking);
                    commonJS.saveLocal("history_info", jsonHistory);
                    UserAction.doAutoSync(jsonRead, jsonTracking, jsonHistory)
                }
            });
        } catch (e) {
            UserAction.extendAutoSync();
        }
    },
    doAutoSync: (readChapter, trackingChapter, historyData) => {
        $.ajax({
            url: jsContext + `/api/user/auto-sync`,
            beforeSend: beforeAuth,
            type: 'POST',
            dataType: "json",
            data: {
                readDataTracking: readChapter,
                trackingData: trackingChapter,
                historyData: historyData
            }
        }).done(res => {
            if (res.status) {
                UserAction.extendAutoSync();
            }
        });
    },
    extendAutoSync: () => {
        commonJS.saveLocal("trigger_auto", new Date().getTime() + 86400000);
    },
    doSyncPush: (_self) => {
        if ($(_self).hasClass(headers.inProgress)) {
            return;
        }

        $(_self).addClass(headers.inProgress);
        // Sync push
        try {
            const status = Notification.permission;
            if (status === "denied") {
                notification.show(commonMessages.pushDenied, notification.Error);
            } else {
                const addFunc = (state, res) => {
                    if (res.status && res.result) {
                        notification.show(commonMessages.pushSuccess, notification.Success);
                    } else {
                        notification.show(res.messages[0], notification.Info);
                    }
                };
                const denied = (state) => {
                    if (state === "denied") {
                        notification.show(commonMessages.pushDenied, notification.Info);
                    }
                };
                registerPushNotification((token) => {
                    return $.ajax({
                        url: jsContext + `/api/push/sync`,
                        beforeSend: beforeAuth,
                        type: 'POST',
                        dataType: "json",
                        data: {comicId: '', token: token, sessionId: commonJS.getLocalSession()}
                    });
                }, addFunc, denied);
            }
        } catch (e) {
            notification.show(commonMessages.pushUnSupport, notification.Info);
        }
        // Sync account
        $.ajax({
            url: jsContext + `/api/user/syncData`,
            beforeSend: beforeAuth,
            type: 'POST',
            dataType: "json",
            data: {localSessionId: commonJS.getLocalSession(), historyData: commonJS.getHistoryData(), trackingData: commonJS.getTrackingData()}
        }).done(res => {
            if (res.status === true) {
                notification.show(commonMessages.syncSuccess, notification.Success);
                commonJS.storeLocalSession(res.result.localSessionId)
            }
            $(_self).removeClass(headers.inProgress);
        });

    },
    disableShowReminderUser: (isRemoved = false) => {
      if (isRemoved) {
          commonJS.removeLocal("block-reminder");
          notification.show(commonMessages.unlockNotificationSuccess, notification.Success);
          return;
      }
      commonJS.saveLocal("block-reminder", true);
    },
    checkingReminderUser: () => {
        $.ajax({
            url: jsContext + `/api/user/getCountReminder`,
            beforeSend: beforeAuth,
            type: 'GET'
        }).done(res => {
            if (res !== null && res.status && res.result > 0) {
                UserAction.doLoadReminder(false);
                if (commonJS.hasLocal("block-reminder")) {
                    return;
                }
                $(headers.classNotiReminderTxt).text(res.result);
                $(headers.classUserReminder).append("<span>(" + res.result + ")</span>");
                diaLogPopup.closeAndOpenDiaLog(headers.idDialogNotificationReminder);
            }
        })
    }
};

const headers = {
    inProgress: "in-progress",
    classBtnLogin: ".btn-menu-login",
    classBtnSearch: ".btn-menu-search",
    classBtnUser: ".btn-menu-user",
    classShowTag: "show-tag",
    classBtnCloseLoginDialog: ".btn-close-dialog-login",
    classBtnCloseSearchDialog: ".btn-close-dialog-search",
    classBtnCloseUserDialog: ".btn-close-dialog-user",
    classBtnCloseRenameDialog: ".btn-close-dialog-rename",
    classBtnCloseReminderDialog: ".btn-close-dialog-reminder",
    classBtnCloseNotiReminderDialog: ".btn-close-dialog-noti-reminder",
    idDialogLogin: "#dialog-login",
    idDialogSearch: "#dialog-search",
    idDialogUser: "#dialog-user",
    idDialogRename: "#dialog-rename",
    idDialogReminder: "#dialog-reminder",
    idDialogNotificationReminder: "#dialog-noti-reminder",
    classInputSearchHeader: "#dialog-search .input-search-dialog-header",
    classDialogBackdrop: ".dialog-backdrop",
    classDialogSearchResult: "#dialog-search .dialog-body .search-result",
    classDialogSearchLoading: "#dialog-search .lds-loading",
    classDialogSearchNodata: "#dialog-search .dialog-body .dialog-no-data",
    currentSearch: undefined,
    classFacebook: ".sign-in-fb",
    classGoogle: ".sign-in-ga",
    btnMenuLogin: ".menu-user-name",
    btnMenuRank: ".menu-user-rank",
    btnMenuUserName: ".menu-user-name > span",
    btnMenuUserRank: ".menu-user-rank > span",
    btnMenuGroupUser: ".menu-group-user",
    btnAdmin: ".menu-user-management",
    classDisabledAdmin: "menu-user-management-disabled",
    classDisabledReminder: "menu-user-block-reminder",
    classBtnMenuUserAction: ".menu-user-action",
    classBtnMenuChangeDesign: ".btn-change-design",
    classBtnMenuFollow: ".menu-user-follow",
    btnMenuUserAction: "menu-user-action",
    dialogInputNameRename: "#dialog-rename .change-name",
    dialogInputTitleRename: "#dialog-rename .change-title",
    dialogBtnRename: "#dialog-rename .btn-rename",
    dialogBtnCancel: "#dialog-rename .btn-cancel",
    dialogBodyReminder: "#dialog-reminder .dialog-body .reminder-list",
    classBtnReminderBlock: "#dialog-reminder .reminder-block",
    classReminderBlock: "reminder-block",
    classBtnNotiReminderCancel: "#dialog-noti-reminder .btn-cancel",
    classBtnNotiReminderOkay: "#dialog-noti-reminder .btn-okay",
    classBtnNotiReminderBlock: "#dialog-noti-reminder .btn-block",
    classActiveGroup: "active-group",
    classNotiReminderTxt: "#dialog-noti-reminder .dialog-body .reminder-noti-msg .num-msg",
    classUserReminder: "#dialog-user .msg-reminder",
    initState: () => {
        headers.checkMessageAll();
        commonJS.checkReportFixed();
        UserAction.checkingReminderUser();
        UserAction.initAutoSync();
        commonJS.checkBannedUser();
    },
    verifyLogin: () => {
        if (commonJS.isAuthenticated()) {
            const user = getUserData();
            $(headers.classBtnUser + " img").attr("src", user.image);
            $(headers.classBtnUser).addClass(headers.classShowTag);
            $(headers.btnMenuUserName).html(user.name);
            $(headers.btnMenuLogin).removeAttr("data-action");
            $(headers.btnMenuUserRank).html(commonMessages.rankerUser + " " + (user.rank + 1));
            if (user.admin || user.team) {
                $(headers.btnAdmin).removeClass(headers.classDisabledAdmin);
            }
            if (commonJS.hasLocal("block-reminder")) {
                $("." + headers.classDisabledReminder).removeClass(headers.classDisabledReminder);
            }
        } else {
            $(headers.classBtnLogin).addClass(headers.classShowTag);
            // Hide menu user
            $(headers.btnMenuRank).hide();
            $(headers.classBtnMenuUserAction).hide();
            $(headers.classBtnMenuChangeDesign).show();
            $(headers.classBtnMenuFollow).hide();
            $(headers.btnMenuGroupUser).hide();
            $(headers.btnMenuGroupUser + "-divider").hide();
        }
    },
    checkMessageAll: () => {
        if (!!commonMessages.messageAll) {
            let messageAll = commonMessages.messageAll.split("||");
            const key = messageAll[0];
            const value = messageAll[1];
            if (!commonJS.getLocal(key)) {
                commonJS.saveLocal(key, value);
                notification.show(value, notification.Info, 7000);
            }
        }
    },
    goToComment: (_self) => {
        const url = $(_self).attr("data-url");
        if (url) {
            doRedirect(url);
        }
    },
    doUserAction: (_self) => {
        let action = $(_self).attr("data-action");
        if (action === "reminder") {
            diaLogPopup.closeAndOpenDiaLog(headers.idDialogUser);
            UserAction.doLoadReminder();
            return;
        }
        if (action === "block-reminder") {
            UserAction.disableShowReminderUser(true);
            return;
        }
        if (action === "rename") {
            UserAction.checkPermissionUpdateName();
            return;
        }
        if (action === "request") {
            UserAction.requestUpgrade(_self);
            return;
        }

        if (action === "auto-sync") {
            UserAction.doActiveAuto();
            return;
        }

        if (action === "sync") {
            const isAgree = confirm(commonMessages.confirmSync);
            if (isAgree) {
                UserAction.doSyncPush(_self);
            }
            return;
        }
        if (action === "restore") {
            UserAction.doRestoreDeviceData(_self);
            return;
        }
        if (action === "ads") {
            UserAction.doSkipAds(_self);
            return;
        }
        if (action === "change-design") {
            UserAction.changeDesign(_self);
            return;
        }
        if (action === "login") {
            diaLogPopup.closeAndOpenDiaLog(headers.idDialogUser);
            diaLogPopup.closeAndOpenDiaLog(headers.idDialogLogin);
            return;
        }
        if (action === "login-pop") {
            diaLogPopup.closeAndOpenDiaLog(headers.idDialogLogin);
            return;
        }
        if (action === "sign-out") {
            UserAction.doSignOut();
            return;
        }
    },
    doSearch: () => {
        if (headers.currentSearch !== undefined) {
            clearTimeout(headers.currentSearch);
        }

        let value = $(headers.classInputSearchHeader).val();
        if (!value || value.length < 2) {
            $(headers.classDialogSearchNodata).addClass("active");
            $(headers.classDialogSearchLoading).removeClass("active");
            $(headers.classDialogSearchResult).html("");
            return;
        }

        $(headers.classDialogSearchLoading).addClass("active");
        $(headers.classDialogSearchNodata).removeClass("active");
        $(headers.classDialogSearchResult).html("");

        headers.currentSearch = setTimeout(() => {
            $.ajax({
                url: jsContext + "/api/comic/search",
                type: 'GET',
                dataType: "json",
                data: {name: value}
            }).done(res => {
                if (res.status && res.result.length > 0) {
                    const data = res.result;
                    for (const index in data) {
                        $(headers.classDialogSearchResult).append(headers.renderHtmlItem(data[index]));
                    }
                    $(headers.classDialogSearchResult).addClass("active");
                } else {
                    $(headers.classDialogSearchNodata).addClass("active");
                    $(headers.classDialogSearchResult).removeClass("active");
                }
                $(headers.classDialogSearchLoading).removeClass("active");
            });
        }, 1000);
    },
    toggleGroup: () => {
        if ($(headers.btnMenuGroupUser).hasClass(headers.classActiveGroup)) {
            $(headers.btnMenuGroupUser).removeClass(headers.classActiveGroup);
            return;
        }
        $(headers.btnMenuGroupUser).addClass(headers.classActiveGroup);
    },
    renderHtmlItem: (item) => {
        if(!item.chapterLatest) {
            return ;
        }
        const urlDetail = commonMessages.comicDetailAppend + item.nameEn;
        const urlView = urlDetail + commonMessages.comicViewAppend + item.chapterLatest[0];
        const titleInfo = item.name + "..." + commonMessages.otherName +`: `+ item.otherName;
        return `<div class="item border-box pa-1">
						<a href="` + urlDetail + `" class="image">
							<img src="` + item.photo + `">
						</a>
						<div class="info" title="`+ titleInfo +`">
							<a href="` + urlDetail + `" class="name">` + item.name + `<div class="otherName">`+ commonMessages.otherName +`: `+ item.otherName +`</div></a>
							<a href="` + urlView + `" class="chapter">` + commonMessages.prefixChapter + ` ` + item.chapterLatest[0] + `</a>
						</div>
					</div>`;
    },
    drawReminderHtml: (data, msg = false) => {
        const mentionUser = commonMessages.mentionReminder.replace("{0}", data.commenter).replace("{1}", data.reminder);
        let url = msg ? "" : `/truyen/` + data.comicNameEn + '?commentId=' + data.commentId + "#comment-main-section";
        if (data.novel) {
            url = msg ? "" : `/tieu-thuyet/` + data.comicNameEn + '?commentId=' + data.commentId + "#comment-main-section";
        }
        const clazzActive = data.watched ? "" : "active";
        let replyBtn = "";
        if (!msg) {
            replyBtn = "<div class='reminder-reply'>" + commonMessages.replyTxt + "</div>"
        }
        return `<div class="reminder-block ` + clazzActive + ` pa-1 mb-1" data-url= ` + url + `>
                <div class="comic">
                    <span class="title">` + data.comicName + `</span>
                </div>
                <span class="date">` + data.createdDate + `</span>
                <div class="reminder">` + mentionUser + `</div>
                <div class="content">` + data.content + `</div>
                ` + replyBtn + `
            </div>`
    }
};

$(document).ready(function () {
    headers.initState();

    $(document).on("click", commonJS.scrollTopClass + "," + commonJS.scrollTopAllClass, function () {
        commonJS.scrollTop();
    });
    $(document).on("click", headers.classBtnCloseLoginDialog, function () {
        diaLogPopup.closeAndOpenDiaLog(headers.idDialogLogin);
    });
    $(document).on("click", headers.classBtnUser + "," + headers.classBtnCloseUserDialog + "," + headers.classBtnLogin, function () {
        diaLogPopup.closeAndOpenDiaLog(headers.idDialogUser);
    });
    $(document).on("click", headers.classBtnCloseReminderDialog, function () {
        diaLogPopup.closeAndOpenDiaLog(headers.idDialogReminder);
    });
    $(document).on("click", headers.classBtnCloseRenameDialog + "," + headers.dialogBtnCancel, function () {
        diaLogPopup.closeAndOpenDiaLog(headers.idDialogRename);
    });
    $(document).on("click", headers.dialogBtnRename, function (e) {
        UserAction.updateUserInformation(e.target);
    });
    $(document).on("click", headers.classBtnSearch + "," + headers.classBtnCloseSearchDialog, function () {
        diaLogPopup.closeAndOpenDiaLog(headers.idDialogSearch);
    });
    $(document).on("click", headers.classBtnCloseNotiReminderDialog + "," + headers.classBtnNotiReminderCancel, function () {
        diaLogPopup.closeAndOpenDiaLog(headers.idDialogNotificationReminder);
    });
    $(document).on("click", headers.classBtnNotiReminderOkay, function () {
        diaLogPopup.closeAndOpenDiaLog(headers.idDialogNotificationReminder);
        UserAction.doLoadReminder(true);
    });
    $(document).on("click", headers.classBtnNotiReminderBlock, function () {
        diaLogPopup.closeAndOpenDiaLog(headers.idDialogNotificationReminder);
        UserAction.disableShowReminderUser();
    });
    $(document).on("click", headers.classFacebook, function () {
        commonJS.doFacebookSignIn();
    });
    $(document).on("click", headers.classGoogle, function () {
        commonJS.doGoogleSignIn();
    });
    $(document).on("click", headers.btnMenuGroupUser, function () {
        headers.toggleGroup();
    });
    $(document).on("keyup", headers.classInputSearchHeader, (e) => {
        headers.doSearch();
    });
    $(document).on("click", headers.classBtnMenuUserAction, (e) => {
        e.stopPropagation();
        if (e.target.classList.contains(headers.btnMenuUserAction)) {
            headers.doUserAction(e.target);
            return;
        }
        headers.doUserAction($(e.target).closest(headers.classBtnMenuUserAction));
    });
    $(document).on("click", headers.btnMenuLogin, (e) => {
        e.stopPropagation();
        if (e.target.classList.contains("menu-user-name")) {
            headers.doUserAction(e.target);
            return;
        }
        headers.doUserAction($(e.target).closest(headers.btnMenuLogin));
    });
    $(document).on("click", headers.classBtnReminderBlock, (e) => {
        e.stopPropagation();
        if (e.target.classList.contains(headers.classReminderBlock)) {
            headers.goToComment(e.target);
            return;
        }
        headers.goToComment($(e.target).closest(headers.classBtnReminderBlock));
    });
});