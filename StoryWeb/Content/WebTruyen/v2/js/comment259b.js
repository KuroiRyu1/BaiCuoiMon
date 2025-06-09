const commentAction = {
    btnCommentClazz: ".btn-comment",
    commentSectionClazz: ".comment-section",
    commentLoading: ".comment-section .lds-loading",
    commentIdFilter: "#comment-id-filter",
    comicId: "#comic-id-comment",
    btnLoadMore: ".btn-more",
    isLoaded: false,
    bufferLoaded: 1000,
    commentScroll: 0,
    page: 0,
    timeout: 2000,
    doComment: (_self) => {
        if ($(_self).hasClass("btn-disable")) {
            return;
        }
        if (!commonJS.isAuthenticated()) {
            notification.show(commentMessages.requiredLogin, notification.Info)
            return;
        }
        let input = $(_self).parent().find(".input-comment").val();
        if (input.length < 5) {
            notification.show(commentMessages.minLength, notification.Warn);
            return;
        }
        const defaultTag = $(_self).parent().find(".input-comment").attr("data-tag");
        if (!!defaultTag && defaultTag !== "" && !input.includes(defaultTag)) {
            input = defaultTag + " " + input;
        }
        const parentId = $(_self).attr("data-id");
        const comicId = $(commentAction.comicId).val();
        const chapterN = $(commentAction.comicId).attr("data-number");
        const type = $(commentAction.comicId).attr("data-type");
        let data = {
            comicId: comicId,
            chapterNumber: chapterN,
            parentCommentId: parentId,
            content: input,
            type: type
        };
        $(_self).addClass("btn-disable");
        $.ajax({
            url: jsContext + `/api/user/addComment`,
            beforeSend: beforeAuth,
            type: 'POST',
            dataType: "json",
            data: data
        }).done(res => {
            if (res.status === true) {
                if (res.result.wait) {
                    notification.show(res.result.message, notification.Warn);
                    setTimeout(() => {
                        $(_self).removeClass("btn-disable");
                    }, 5000);
                    return;
                }
                const commentSection = $(_self).parent();
                const isReply = (commentSection).hasClass("input-2");
                if (isReply) {
                    const u = {
                        username: commentMessages.username,
                        content: commentAction.changeIdToNameComment(commentSection, input),
                        dateTime: commentMessages.datetime,
                        owner: true
                    };
                    $(commentSection).parent().parent().append(commentAction.buildChildCommentHtml(u, false, true));
                    $(commentSection).closest(".active").removeClass("active")
                    $(commentSection).remove();
                } else {
                    $(_self).prev().val("");
                }
                notification.show(res.result.message, notification.Success);
            } else {
                notification.show(res.messages[0], notification.Error);
            }
            $(_self).removeClass("btn-disable");
        })
    },
    loadComment: (isLoadMore = false) => {
        const comicId = $(commentAction.comicId).val();
        const chapterN = $(commentAction.comicId).attr("data-number");
        const commentId = $(commentAction.comicId).attr("data-commentId")
        let data = {
            value: comicId,
            extraData: chapterN,
            p: commentAction.page,
            commentId: commentId
        };
        $.ajax({
            url: jsContext + `/api/comic/comments`,
            beforeSend: beforeAuth,
            type: 'GET',
            dataType: "json",
            data: data
        }).done(res => {
            if (isLoadMore) {
                $(commentAction.btnLoadMore).remove();
            }
            if (res.status === true) {
                const data = res.result.comments;
                if (data.length > 0) {
                    let html = "";
                    for (var i = 0; i < data.length; i++) {
                        html += commentAction.buildCommentHTML(data[i], res.result.admin);
                    }
                    $(commentAction.commentSectionClazz).append(html);
                }
                if (res.result.hasMore) {
                    $(commentAction.commentSectionClazz).append(commentAction.renderLoadMore());
                }
            }
            $(commentAction.commentLoading).hide();
        })
    },
    doDelete: (_self) => {
        const data = commentAction.getDataComment(_self);
        $.ajax({
            url: jsContext + `/api/user/deleteComment`,
            beforeSend: beforeAuth,
            type: 'POST',
            dataType: "json",
            data: data
        }).done(res => {
            if (res.status === true) {
                notification.show(commentMessages.deleteSuccess, notification.Success);
                if (isChildComment) {
                    $(_self).parent().remove();
                } else {
                    $(_self).parent().parent().remove();
                }
            } else {
                notification.show(res.messages[0], notification.Error);
            }
        })
    },
    doSpoil: (_self) => {
        const data = commentAction.getDataComment(_self);
        $.ajax({
            url: jsContext + `/api/user/spoilComment`,
            beforeSend: beforeAuth,
            type: 'POST',
            dataType: "json",
            data: data
        }).done(res => {
            if (res.status === true) {
                notification.show(commentMessages.hideSpoil, notification.Success);
            } else {
                notification.show(res.messages[0], notification.Error);
            }
        })
    },
    doAttached: (_self) => {
        const data = commentAction.getDataComment(_self);
        $.ajax({
            url: jsContext + `/api/user/attachComment`,
            beforeSend: beforeAuth,
            type: 'POST',
            dataType: "json",
            data: data
        }).done(res => {
            if (res.status === true) {
                notification.show(commentMessages.attached, notification.Success);
            } else {
                notification.show(res.messages[0], notification.Error);
            }
        })
    },
    doReportComment: (_self) => {
        const isAgree = confirm(commentMessages.reportWarn);
        if (isAgree) {
            const isRoot = $(_self).parent().hasClass("action");
            const index = isRoot ? $(_self).parent().parent().attr("data-index") : $(_self).parent().attr("data-index");
            const commentId = isRoot ? $(_self).parent().parent().attr("data-id") : $(_self).closest(".comment").attr("data-id");
            $.ajax({
                url: jsContext + `/api/user/reportComment`,
                beforeSend: beforeAuth,
                type: 'POST',
                dataType: "json",
                data: {index, commentId}
            }).done(res => {
                if (res.status === true) {
                    notification.show(commentMessages.reportComment, notification.Success);
                } else {
                    notification.show(commonMessages.unknownError, notification.Error);
                }
            })
        }
    },
    doReply: (_self) => {
        if (!commonJS.isAuthenticated()) {
            notification.show(commentMessages.requiredLogin, notification.Info)
            return;
        }
        $(_self).closest(".comment").find(".input-2").remove();
        const _parent = $(_self).parent();
        const hasReply = $(_parent).find(".input");
        $(_self).closest(".comment").addClass("active");
        if (hasReply.length === 0) {
            const commentId = $(_self).attr("data-id");
            let tagName = $(_self).parent().find(".btn-tag").attr("data-n");
            if (!tagName) {
                tagName = "";
            } else {
                tagName = "@@" + tagName + "@@";
            }

            let name = "";
            if ($(_parent).hasClass("action")) {
                name = $(_parent).closest(".comment").children(".user").find(".name").text();
            } else {
                name = $(_parent).find(".user .name").text();
            }

            $(_parent).append(commentAction.buildInputHtml(commentId, tagName, name));
        }

    },
    addTag: (_self) => {
        const name = $(_self).attr("data-n");
        const input = $(_self).closest(".comment").find(".input-comment");
        const value = $(input).val() + " @@" + name + "@@ ";
        $(input).val(value);
    },
    viewSpoilContent: (_self) => {
        const content = $(_self).attr("data-content");
        $(_self).closest("div").html(content);
    },
    buildCommentHTML: (comment, isAdmin = false) => {
        let removeHtml = commentAction.buildActionHtml(comment, comment.id, isAdmin);
        const ownerHtml = comment.owner ? "your" : "";
        let commentContent = commentAction.buildCommentContent(comment);

        let htmlChapter = comment.chapterNumber !== "" ? `<span class="chapter">` + commentMessages.prefixChapter + ` ` + comment.chapterNumber + `</span>` : "";
        let iconAttached = comment.attached ? `<svg class="attach-icon MuiSvgIcon-root" focusable="false" aria-hidden="true" viewBox="0 0 24 24" data-testid="PushPinIcon" tabindex="-1" title="PushPin"><path fill-rule="evenodd" d="M16 9V4h1c.55 0 1-.45 1-1s-.45-1-1-1H7c-.55 0-1 .45-1 1s.45 1 1 1h1v5c0 1.66-1.34 3-3 3v2h5.97v7l1 1 1-1v-7H19v-2c-1.66 0-3-1.34-3-3z"></path></svg>` : "";
        let html = `<div class="comment" data-index="` + 0 + `" data-id="` + comment.id + `"><div class="user"><span class="name ` + ownerHtml + `">` + iconAttached + comment.username + " " + htmlChapter + `</span><span class="rank rank-` + comment.rank + `">` + comment.title + `</span><span class="time">` + comment.dateTime + `</span></div><div class="p-comment zs-bg-3 v-sheet theme-dark">` + commentContent + `</div><div class="action"> ` + removeHtml + `</div>`;
        if (!!comment.childrenComments) {
            const childComments = comment.childrenComments;
            const len = childComments.length;
            const hideChildComment = len > 3;
            if (hideChildComment) {
                html += `<div class="hide-comment">Hiển thị ` + (len - 2) + ` bình luận cũ</div>`;
            }
            for (let i = 0; i < len; i++) {
                childComments[i].index = i + 1;
                let clazzHide = "";
                if (hideChildComment && i < len - 2) {
                    clazzHide = "hide";
                }
                html += commentAction.buildChildCommentHtml(childComments[i], comment.id, clazzHide, isAdmin);
            }
        }
        html += "</div>";
        return html;
    },
    buildChildCommentHtml: (comment, parentId, clazzHide, isAdmin = false, temp = false) => {
        let removeHtml = commentAction.buildActionHtml(comment, parentId, isAdmin, temp);
        let title = "";
        if (!!comment.title) {
            title = comment.title;
        }
        const ownerHtml = comment.owner ? "your" : "";
        let childrenHTML = `<div class="c-comment ` + clazzHide + `" data-index="` + comment.index + `"  data-id="` + comment.id + `"><div class="user"><span class="name ` + ownerHtml + `">` + comment.username + `</span><span class="rank rank-` + comment.rank + `">` + title + `</span><span class="time">` + comment.dateTime + `</span></div><div class="comment zs-bg-3 v-sheet theme-dark">` + commentAction.buildCommentContent(comment) + `</div> ` + removeHtml + `</div>`;
        return childrenHTML;
    },
    buildActionHtml: (comment, parentId, isAdmin = false, temp = false) => {
        let removeHtml = "";
        removeHtml += `<span class="btn-reply" data-id="` + parentId + `">` + commentMessages.txtReply +`</span>`;
        if (!temp && (isAdmin || comment.owner)) {
            removeHtml += `<span class="btn-del">` + commentMessages.txtDelete + ` </span>`;
        }
        if (isAdmin) {
            removeHtml += `<span class="btn-spoil">` + commentMessages.txtSpoil + ` </span>`;
            removeHtml += `<span class="btn-attached">` + commentMessages.txtAttach + ` </span>`;
        }
        if (!comment.owner) {
            removeHtml += `<span class="btn-report-cmt">` + commentMessages.txtReport + ` </span><span class="btn-tag" data-n="` + comment.id + `">` + commentMessages.txtTag + ` </span>`;
        }
        return removeHtml;
    },
    buildCommentContent: (comment) => {
        if (comment.spoil) {
            return commentMessages.txtSpoilContent.replace("{0}", comment.content);
        }
        return comment.content;
    },
    buildInputHtml: (commentId, tagName, name) => {
        if (tagName === "") {
            name = "" + commentMessages.txtPlaceHolder;
        } else {
            name = commentMessages.replyTo.replace("{0}", name);
        }
        return `<div class="input input-2"><textarea class="input-comment" data-tag="`+ tagName +`" placeholder="` + name + `" maxlength="500"></textarea><button data-id="` + commentId + `" class="btn-comment mx-1 rounded-sm zs-bg-3 mr-4 btn-default btn-default-is-elevated theme-dark btn-size-default">` + commentMessages.txtBtnSend + `</button></div>`
    },
    renderLoadMore: () => {
        return `<div class="btn-more btn-style-dash rounded-sm zs-bg-3 theme-dark">` + commentMessages.txtBtnMore + ` </div>`;
    },
    getDataComment: (_self) => {
        let childId = "";
        const isChildComment = $(_self).parent().hasClass("c-comment");
        if (isChildComment) {
            childId = $(_self).parent().attr("data-id");
        }
        const data = {
            id: $(_self).parent().parent().attr("data-id"),
            childId: childId,
        };
        return data;
    },
    showHiddenComment: (_self) => {
        $(_self).parent().find(".hide").removeClass("hide");
        $(_self).remove();
    },
    changeIdToNameComment: (_self, content) => {
        const mapIdToName = {};
        const parentComment = $(_self).closest(".comment");
        let commentName = $(parentComment).find("> .user > .name").text();
        if (commentName.indexOf("Chương") !== -1) {
            commentName = commentName.substring(0, commentName.indexOf("Chương"));
        }
        if (commentName.indexOf("Chapter") !== -1) {
            commentName = commentName.substring(0, commentName.indexOf("Chapter"));
        }
        mapIdToName[$(parentComment).attr("data-id")] = commentName;

        const childComments = $(parentComment).find(".c-comment");
        for (let i = 0; i < childComments.length; i++) {
            mapIdToName[$(childComments[i]).attr("data-id")] = $(childComments[i]).find("> .user > .name").text();
        }

        var reg = /@@(.*?)@@/;
        var match = reg.exec(content);
        while (!!match) {
            content = content.replaceAll(match[0], "<span class='link-tag'>#" + mapIdToName[match[1]] + "</span>");
            match = reg.exec(content);
        }
        return content;
    },
    checkLoadComment: () => {
        if (commentAction.isLoaded) {
            return;
        }
        try {
            commentAction.commentScroll = window.scrollY || document.documentElement.scrollTop;
            if (commentAction.commentScroll + window.innerHeight + commentAction.bufferLoaded > $(commentAction.commentSectionClazz).offset().top) {
                // console.log("LOADED");
                commentAction.loadComment();
                commentAction.isLoaded = true;
            }
        } catch (e) {
            commentAction.loadComment();
            commentAction.isLoaded = true;
        }
    }
};
$(document).ready(function () {
    setTimeout(() => {commentAction.checkLoadComment();}, commentAction.timeout);
    $(document).on("click", ".btn-comment", function () {
        commentAction.doComment(this);
    });
    $(document).on("click", ".btn-reply", function (e) {
        e.stopPropagation();
        commentAction.doReply(this);
    });
    $(document).on("click", ".btn-del", function (e) {
        e.stopPropagation();
        commentAction.doDelete(this);
    });
    $(document).on("click", ".btn-spoil", function (e) {
        e.stopPropagation();
        commentAction.doSpoil(this);
    });
    $(document).on("click", ".btn-attached", function (e) {
        e.stopPropagation();
        commentAction.doAttached(this);
    });
    $(document).on("click", ".btn-tag", function (e) {
        e.stopPropagation();
        commentAction.addTag(this);
    });
    $(document).on("click", ".btn-report-cmt", function (e) {
        e.stopPropagation();
        commentAction.doReportComment(this);
    });
    $(document).on("click", ".view-comment-spoil", function (e) {
        e.stopPropagation();
        commentAction.viewSpoilContent(this);
    });
    $(document).on("click", commentAction.btnLoadMore, function () {
        $(this).html(`<div class="lds-loading"><div></div><div></div><div></div></div>`);
        commentAction.page += 1;
        commentAction.loadComment(true);
    });
    $(document).on("click", ".hide-comment", function () {
        commentAction.showHiddenComment(this);
    });
    try {
        window.addEventListener('scroll', () => {commentAction.checkLoadComment();});
    } catch (e) {
    }
});