messaging.onMessage((payload) => {
    notification.showPush(payload.data);
});