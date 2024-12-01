import {Button} from "@/components/ui/button.tsx";
import {Card} from "@/components/ui/card.tsx";
import {List, Settings} from "lucide-react";
import React, {useEffect, useState} from "react";
import {useTranslation} from "react-i18next";

export function SideBar({ className }: { className?: string }) {
    "use client";


    const { t } = useTranslation();
    const appApi = (window as any).chrome.webview.hostObjects.appApi;
    const [appVersion, setAppVersion] = useState<string>("");

    useEffect(() => {
        const fetchAppVersion = async () => {
            const version = await appApi.GetAppVersion();
            setAppVersion(version);
        };

        fetchAppVersion();
    }, []);
    

    return (
        <>
            <Card
                className={`${className} flex w-auto max-w-80 p-2 shadow-xl shadow-primary/5 ml-4 my-4 shrink-0 overflow-auto`}
            >
                <div className="flex flex-col gap-1 p-2 min-w-40 flex-grow">
                    <SideBarItem href={"/"} text={"Home"} icon={List} />
                    <SideBarItem href={"/settings"} text={t("MAIN.SETTINGS")} icon={Settings} />
                    <Button
                        variant={"ghost"}
                        className={
                            "text-sm justify-start hover:bg-card hover:text-card-foreground mt-auto"
                        }
                    >
                        {appVersion ? `${appVersion}` : "Loading..."}
                    </Button>
                </div>
            </Card>
        </>
    );
}


function SideBarItem({
                         href,
                         text,
                         icon,
                     }: {
    href: string;
    text: React.ReactNode;
    icon: React.ComponentType<{ className?: string }>;
}) {
    const IconElement = icon;
    return (
        <Button
            variant="ghost"
            className="justify-start flex-shrink-0 w-full"
            onClick={() => window.location.hash = href}
        >
            <div className="mr-4">
                <IconElement className="h-5 w-5" />
            </div>
            {text}
        </Button>
    );
}

