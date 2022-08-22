<p align="center">
  <a href="https://www.advantshop.net" target="_blank">
    <img src='./img/Logo.svg' width="100%" alt="ADVANTSHOP">
  </a>
</p>
<div align="center">
  <img src='https://img.shields.io/badge/ADVANTSHOP-10.0.12-0b88f2?style=for-the-badge' alt="ADVANTSHOP-11.0">
  <p align="center">  
    <img src='https://img.shields.io/badge/.NET Framework-4.5.2-512BD4?logo=.NET' alt=".NET Framework-4.5.2">
    <img src='https://img.shields.io/badge/Angular JS-1.8.3-dd1b16?logo=angular' alt="">
  </p>
</div>

## Webpack

<details>
<summary>
<b>
Обобщенная информация о командах
</b>
</summary>

- Сборка в режиме продакшена
    ```sh
    npm run build
    ```
- Сборка в режиме разработки
    ```sh
    npm run dev
    ```
- Сборка critical css
    ```sh
    npm run criticalcss
    ```
- Сборка в режиме разработки с включенным режимом watch (отслеживание изменений)
    ```sh
    npm run start
    ```
- Запуск тестов
    ```sh
    npm run test
    ```
- Сборка стилей в воронках
    ```sh
    npm run landing-styles
    ```
- Сборка стилей в админской части
    ```sh
    npm run admin-styles
    ```
</details>
<br>
<details>
<summary>
<b>
Инструкция по сборке основной части фронтенда
</b>
</summary>

1. Выполнить чистую установку пакетов

```sh
npm install
```

2. Вызвать сборку

```sh
npm run <mode>

Options:
  mode            Bundle mode: development or production    [choices: "dev", "build"]
```

</details>
<br>
<details>
<summary>
<b>
Инструкция по сборке модулей
</b>
</summary>

1. Выполнить чистую установку пакетов

```sh
npm install
```

2. Вызвать сборку модулей

```sh
npm run build -- <options>

Options:
  -b                Сборка всех модулей
  -c <ModuleName>   Собрать определенный модуль
```

</details>
<br>
<details>
<summary>
<b>
Инструкция по сборке шаблонов
</b>
</summary>

1. Выполнить чистую установку пакетов

```sh
npm install
```

2. Вызвать сборку основной части фронтенда

```sh
npm run build
```

3. Вызвать сборку шаблонов

```sh
npm run build -- <options>

Options:
  -a                  Сборка всех шаблонов
  -l <TemplateName>   Собрать определенный шаблон
```

</details>
