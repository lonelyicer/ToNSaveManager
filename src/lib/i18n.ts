import { initReactI18next } from "react-i18next";
import de_DEJson from "../../Localization/Language/de-DE.json";
import en_USJson from "../../Localization/Language/en-US.json";
import es_ESJson from "../../Localization/Language/es-ES.json";
import fr_FRJson from "../../Localization/Language/fr-FR.json";
import it_ITJson from "../../Localization/Language/it-IT.json";
import ja_JPJson from "../../Localization/Language/ja-JP.json";
import zh_CNJson from "../../Localization/Language/zh-CN.json";
import zh_TWJson from "../../Localization/Language/zh-TW.json";
import i18n from "i18next";

export const languageResources = {
    de_DE: {
        translation: de_DEJson
    },
    en_US: {
        translation: en_USJson
    },
    es_ES: {
        translation: es_ESJson
    },
    fr_FR: {
        translation: fr_FRJson
    },
    it_IT: {
        translation: it_ITJson
    },
    ja_JP: {
        translation: ja_JPJson
    },
    zh_CN: {
        translation: zh_CNJson
    },
    zh_TW: {
        translation: zh_TWJson
    }
};

export const languages = Object.keys(languageResources);

i18n
    .use(initReactI18next)
    // init i18next
    // for all options read: https://www.i18next.com/overview/configuration-options
    .init({
        debug: true,
        lng: 'en_US',
        fallbackLng: 'en_US',
        interpolation: {
            escapeValue: false, // not needed for react as it escapes by default
        },
        resources: languageResources,
    });

export default i18n;
