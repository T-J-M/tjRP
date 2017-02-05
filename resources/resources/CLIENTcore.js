var anim_menu = API.createMenu("Animation List", "Category", 0, 0, 6);
anim_menu.ResetKey(menuControl.Back);

anim_menu.AddItem(API.createMenuItem("Sitting", "Name(s): sit"));
anim_menu.AddItem(API.createMenuItem("Standing", "Name(s): clean, clipboard1, clipboard2, clipboard3"));
anim_menu.AddItem(API.createMenuItem("Phone", "Name(s): phone1, phone2, phone3"));
anim_menu.AddItem(API.createMenuItem("Ground", "Name(s): checkbody1, checkbody2"));
anim_menu.AddItem(API.createMenuItem("Leaning", "Name(s): lean, leanfoot, leancar"));
anim_menu.AddItem(API.createMenuItem("Surrender", "Name(s): handsup, handsupknees"));
anim_menu.AddItem(API.createMenuItem("Smoking", "Name(s): smoke1, smoke2"));
anim_menu.AddItem(API.createMenuItem("Drinking", "Name(s): coffee1, coffee2"));
anim_menu.AddItem(API.createMenuItem("Social", "Name(s): guitar, drums"));
anim_menu.AddItem(API.createMenuItem("Exit", "Close menu"));

anim_menu.OnItemSelect.connect(function (sender, item, index) {
    API.showCursor(false);
    anim_menu.Visible = false;
});

API.onServerEventTrigger.connect(function (eventName, args) {
    switch (eventName) {
        case 'anim_list':
            API.sendChatMessage("anim_list_call");
            API.showCursor(true);
            anim_menu.Visible = true;
            break;
    }
});

API.onUpdate.connect(function () {
    API.drawMenu(anim_menu);
});

API.onKeyUp.connect(function (sender, e) {
    if (e.KeyCode === Keys.J) {
        API.triggerServerEvent("indicator_left");
    }
    else if (e.KeyCode === Keys.K) {
        API.triggerServerEvent("indicator_right");
    }
});