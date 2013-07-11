/*\
Animate collapsible set;
\*/
$(document).one("pagebeforechange", function() {

    // animation speed;
    var animationSpeed = 200;

    function animateCollapsibleSet(elm) {

        // can attach events only one time, otherwise we create infinity loop;
        elm.one("expand", function() {

            // hide the other collapsibles first;
            $(this).parent().find(".ui-collapsible-content").not(".ui-collapsible-content-collapsed").trigger("collapse");

            // animate show on collapsible;
            $(this).find(".ui-collapsible-content").slideDown(animationSpeed, function() {

                // trigger original event and attach the animation again;
                animateCollapsibleSet($(this).parent().trigger("expand"));
            });

            // we do our own call to the original event;
            return false;
        }).one("collapse", function() {

            // animate hide on collapsible;
            $(this).find(".ui-collapsible-content").slideUp(animationSpeed, function() {

                // trigger original event;
                $(this).parent().trigger("collapse");
            });

            // we do our own call to the original event;
            return false;
        });
    }

    // init;
    animateCollapsibleSet($("[data-role='collapsible-set'] > [data-role='collapsible']"));
});