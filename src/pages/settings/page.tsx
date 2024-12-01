import {ScrollArea} from "@/components/ui/scroll-area.tsx";
import {Card} from "@/components/ui/card.tsx";
import {useTranslation} from "react-i18next";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import {languages} from "@/lib/i18n";

const Settings = () => {
    const {t} = useTranslation();
    return (
        <ScrollArea className={"-mr-3 pr-3 w-full h-full"}>
            <main className="flex flex-col gap-2 flex-shrink flex-grow">
                <Card className={"flex-shrink-0 p-4"}>
                    <p className={"font-black"}>{t("MAIN.SETTINGS")}</p>
                </Card>
                <AppearanceCard />
            </main>
        </ScrollArea>
    )
}

export default Settings

function LanguageSelector() {
    const { i18n } = useTranslation();
    const {t} = useTranslation();

    const handleLanguageChange = (value: string) => {
        i18n.changeLanguage(value);
    };

    return (
        <div className="flex-grow">
            <span className="font-medium">
				{t("SETTINGS.LANGUAGE")}
			</span>
            <Select onValueChange={handleLanguageChange} defaultValue={i18n.language}>
                <SelectTrigger>
                    <SelectValue />
                </SelectTrigger>
                <SelectContent>
                    {languages.map((langCode) => (
                        <SelectItem key={langCode} value={langCode}>
                            {t("DISPLAY_NAME", { lng: langCode })}
                        </SelectItem>
                    ))}
                </SelectContent>
            </Select>
        </div>
    );
}

function AppearanceCard() {
    const {t} = useTranslation();
    return (
        <Card className={"flex-shrink-0 p-4"}>
            <p className={"font-bold"}>{t("SETTINGS.APPEARANCE")}</p>
            <LanguageSelector />
        </Card>
    );
}