export default {
    iconLinks: [
        {
            icon: 'github',
            href: 'https://github.com/gehongyan/Kook.Net',
            title: 'GitHub'
        },
        {
            icon: 'box-seam-fill',
            href: 'https://www.nuget.org/packages/Kook.Net/',
            title: 'NuGet'
        },
        {
            icon: 'chat',
            href: 'https://kook.top/EvxnOb',
            title: 'KOOK'
        }
    ],
    start: () =>
    {
        let target = document.getElementById("toc");

        if (!target) return;

        let config = { attributes: false, childList: true, subtree: true };
        let observer = new MutationObserver((list) =>
        {
            for (const mutation of list)
            {
                if (mutation.type === "childList" && mutation.target == target)
                {
                    let scrollValue = localStorage.getItem("tocScroll");

                    // Add event to store scroll pos.
                    let tocDiv = target.getElementsByClassName("flex-fill")[0];

                    tocDiv.addEventListener("scroll", (event) =>
                    {
                        if (event.target.scrollTop >= 0)
                        {
                            localStorage.setItem("tocScroll", event.target.scrollTop);
                        }
                    });

                    if (scrollValue && scrollValue >= 0)
                    {
                        tocDiv.scroll(0, scrollValue);
                    }

                    observer.disconnect();
                    break;
                }
            }
        });

        observer.observe(target, config);
    }
}
