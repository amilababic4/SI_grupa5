(function () {
    "use strict";
    if (!Auth.guard(["Administrator"])) return;
})();
