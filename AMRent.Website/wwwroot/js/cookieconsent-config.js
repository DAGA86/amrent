import 'https://cdn.jsdelivr.net/gh/orestbida/cookieconsent@3.0.1/dist/cookieconsent.umd.js';

function updateGtagConsent() {
    if (typeof gtag != "undefined") {
        gtag('consent', 'update', {
            'ad_storage': CookieConsent.acceptedCategory('marketing') ? 'granted' : 'denied',
            'ad_user_data': CookieConsent.acceptedCategory('marketing') ? 'granted' : 'denied',
            'ad_personalization': CookieConsent.acceptedCategory('marketing') ? 'granted' : 'denied',
            'analytics_storage': CookieConsent.acceptedCategory('analytics') ? 'granted' : 'denied',
            'functionality_storage': CookieConsent.acceptedCategory('functionality') ? 'granted' : 'denied',
            'personalization_storage': CookieConsent.acceptedCategory('preferences') ? 'granted' : 'denied',
        });
    }
}

CookieConsent.run({
    guiOptions: {
        consentModal: {
            layout: "bar",
            position: "bottom",
            equalWeightButtons: true,
            flipButtons: false
        },
        preferencesModal: {
            layout: "box",
            position: "right",
            equalWeightButtons: true,
            flipButtons: true
        }
    },

    // Update Google Consent Mode v2
    onConsent: () => {

        updateGtagConsent();
    },

    onChange: ({ changedCategories }) => {
        updateGtagConsent();
    },

    categories: {
        necessary: {
            enabled: true,  // this category is enabled by default
            readOnly: true  // this category cannot be disabled
        },
        preferences: {
            enabled: true,
            readOnly: false,
        },
        functionality: {
            enabled: true,
            readOnly: false,
        },
        analytics: {
            enabled: true,
            readOnly: false,
        },
        marketing: {
            enabled: true,
            readOnly: false,
        }
    },
    language: {
        default: "pt",
        autoDetect: "document",
        translations: {
            pt: {
                consentModal: {
                    title: "Damos valor à sua privacidade",
                    description: "As cookies são usadas para lhe proporcionar uma melhor experiência no nosso site, fornecer serviços e ferramentas de terceiros, ajudar-nos a compreender melhor a sua utilização, melhorar o seu funcionamento e para realizar publicidade mais segmentada. Recomendamos que ative todas as cookies, mas se não concordar, pode simplesmente editá-las através da opção de edição disponível. Os detalhes completos estão disponíveis nas Definições de Cookies.",
                    acceptAllBtn: "Aceitar todos",
                    acceptNecessaryBtn: "Rejeitar todos",
                    showPreferencesBtn: "Gerir preferências",
                },
                preferencesModal: {
                    title: "Gerir preferências de cookies",
                    acceptAllBtn: "Aceitar todos",
                    acceptNecessaryBtn: "Rejeitar todos",
                    savePreferencesBtn: "Guardar preferências",
                    closeIconLabel: "Fechar",
                    serviceCounterLabel: "Serviço|Serviços",
                    sections: [
                        {
                            title: "Sobre a sua privacidade",
                            description: "Quando visita um website, este pode armazenar ou recolher informações no seu navegador, principalmente na forma de cookies. Esta informação pode ser sobre si, as suas preferências ou o seu dispositivo e é utilizada principalmente para fazer o website funcionar conforme o esperado. A informação normalmente não o identifica diretamente, mas pode dar-lhe uma experiência web mais personalizada. Uma vez que respeitamos o seu direito à privacidade, pode optar por não permitir alguns tipos de cookies. Clique nos cabeçalhos das diferentes categorias para saber mais e alterar as nossas configurações predefinidas. No entanto, o bloqueio de alguns tipos de cookies pode afetar a sua experiência no website e os serviços que podemos oferecer."
                        },
                        {
                            title: "Tecnicamente necessários <span class=\"pm__badge\">Sempre ativos</span>",
                            description: "Estes cookies são essenciais para os sites de forma a que as suas características funcionem adequadamente. Ex.: cookies de autenticação.",
                            linkedCategory: "necessary"
                        },
                        {
                            title: 'Preferências',
                            description: 'Os cookies de preferência permitem que um website memorize as informações que mudam o comportamento ou o aspeto do website, como o seu idioma preferido ou a região em que se encontra.',
                            linkedCategory: 'preferences'
                        },
                        {
                            title: "Funcionais",
                            description: "Estes cookies permitem-nos melhorar o seu conforto e acessibilidade nos sites, bem como a disponibilização de algumas funcionalidades. Ex.: os cookies funcionais podem ser utilizados para armazenar resultados de pesquisa, língua, dimensões dos caracteres.",
                            linkedCategory: "functionality"
                        },
                        {
                            title: "Performance",
                            description: "Estes cookies recolhem informação sobre como o utilizador usa o site. Os cookies de performance permitem-nos, por exemplo, identificar áreas particularmente populares do nosso site. Desta forma, podemos adaptar o conteúdo dos nossos sites especificamente às suas necessidades e dessa forma melhorar a nossa oferta para si.",
                            linkedCategory: "analytics",
                        },
                        {
                            title: "Publicidade",
                            description: "Estes cookies são utilizados para enviar publicidade e informação promocional que seja relevante para si, por ex., tendo por referência as páginas que visita.",
                            linkedCategory: "marketing"
                        }
                    ]
                }
            }
        }
    },
    disablePageInteraction: true
});