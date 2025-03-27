function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

function log(msg) {
    console.log(msg);
}

async function waitForSelector(selector, time) {
    do {
        const element = document.querySelector(selector);
        if (!element) {
            log(`未找到 ${selector} 元素,等待 ${time} ms 后重试`);
            await sleep(time);
            continue;
        } else {
            log(`找到 ${selector} 元素`)
            return element;
        }
    } while (true);
}

async function waitForSelectorInElement(elementSelector, subSelector, time) {
    do {
        const element = document.querySelector(elementSelector).querySelector(subSelector);
        if (!element) {
            //log(`未找到 ${elementSelector} -> ${subSelector} 元素,等待 ${time} ms 后重试`);
            await sleep(time);
            continue;
        } else {
            //log(`找到 ${elementSelector} -> ${subSelector} 元素`)
            return element;
        }
    } while (true);
}

function getFormattedDate() {
    const now = new Date();
    const year = now.getFullYear();
    const month = String(now.getMonth() + 1).padStart(2, '0'); // 补零
    const day = String(now.getDate()).padStart(2, '0');
    const hours = String(now.getHours()).padStart(2, '0');
    const minutes = String(now.getMinutes()).padStart(2, '0');
    const seconds = String(now.getSeconds()).padStart(2, '0');

    return `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`;
}

async function fetchdata() {
    let buttons = document.querySelectorAll('body > div.flex.min-h-screen.flex-col > main > div.relative.flex.flex-wrap.justify-center.gap-2.p-2 > button');
    let versions = []
    for (let i = 0; i < buttons.length; i++) {
        //主版本
        let button = buttons[i];
        let buttonText = button.innerText;
        log(`抓取 ${buttonText}`);
        let vr = {
            versions: {},
            webMajorVersionText: buttonText,
            snapshotAt:getFormattedDate()
        }
        await sleep(3000);
        button.click();
        //等待加载版本表格
        const tableSelector = 'body > div.flex.min-h-screen.flex-col > main > section > div > div > div.mt-8.flow-root > div > div > table';
        let table = await waitForSelector(tableSelector, 2000);
        if (!table) {
            debugger
        }
        const downloadButtonSelector = 'a[href^="unityhub://"]';
        //表格内的数据是异步加载,所以此处等待表格数据就绪
        await waitForSelectorInElement(tableSelector, downloadButtonSelector, 2000);
        //table可能变化,所以重新获取
        table = await waitForSelector(tableSelector, 2000);
        
        const rows = table.querySelectorAll('tr');
        for (let j = 0; j < rows.length; j++) {
            const row = rows[j];
            const tds = row.querySelectorAll('td');
            //跳过表头
            if (tds.length <= 0) continue;
            let downloadElement = row.querySelector(downloadButtonSelector);
            if (!downloadElement) {
                debugger
            }
            let href = downloadElement.href;
            let releaseDate = await row.querySelector("td:nth-child(2) > div > span");
            let releaseDateText = releaseDate.innerText;
            const match = href.match(/unityhub:\/\/(?<version>.*)\/(?<hash>.*)/);
            const version = match.groups.version;
            const hash = match.groups.hash;

            const item = {
                releaseDate: releaseDateText,
                hash,
                version,
                linux: `https://download.unity3d.com/download_unity/${hash}/LinuxEditorInstaller/Unity.tar.xz`,
                mac: `https://download.unity3d.com/download_unity/${hash}/MacEditorInstaller/Unity-${version}.pkg`,
                macArm64: `https://download.unity3d.com/download_unity/${hash}/MacEditorInstallerArm64/Unity-${version}.pkg`,
                win64: `https://download.unity3d.com/download_unity/${hash}/Windows64EditorInstaller/UnitySetup64-${version}.exe`
            }
            vr.versions[version] = item;
        }
        versions.push(vr);
        //console.log(vr)
    }
    console.log('all done,json=');
    console.log(JSON.stringify(versions))
}

await fetchdata();