# 🗣️ Sistema de Gestão de Ouvidoria

![Badge Status](https://img.shields.io/badge/Status-Concluido-green)
![Badge .NET](https://img.shields.io/badge/Backend-.NET-purple)
![Badge Type](https://img.shields.io/badge/Focus-Customer_Success-orange)

> **Organização, transparência e eficiência no atendimento aos beneficiários.**

Este sistema foi desenvolvido para modernizar o setor de Ouvidoria, permitindo um controle híbrido de atendimentos (agendados e espontâneos) e fornecendo métricas precisas para a tomada de decisão da gestão.

---

## 🎯 Objetivo
Centralizar todo o ciclo de vida do atendimento presencial da ouvidoria, desde a disponibilização de agenda até a análise de satisfação e volumetria, garantindo que nenhum beneficiário fique sem registro.

---

## ✨ Funcionalidades Principais

O sistema resolve quatro dores principais da gestão de atendimento:

### 1. 📅 Gestão de Agenda e Disponibilidade
O administrador tem total controle sobre o calendário.
* **Criação de Slots:** Defina dias e faixas de horários disponíveis para atendimento.
* **Bloqueios:** Impeça agendamentos em feriados ou períodos de ausência da equipe.
* **Agendamento:** Interface para marcar o atendimento do beneficiário no horário desejado.

### 2. ✅ Controle de Execução (Check-in/Feedback)
Não basta agendar, é preciso saber o que aconteceu.
* **Status do Atendimento:** Registre se o beneficiário **Compareceu**, **Faltou** ou **Cancelou**.
* **Feedback Qualitativo:** Campo dedicado para registrar como foi o atendimento, qual a demanda principal e a solução proposta inicial.

### 3. 🚶 Atendimento Espontâneo (Walk-in)
Para os casos onde o beneficiário chega sem aviso prévio.
* **Registro Rápido:** Um fluxo simplificado para cadastrar atendimentos de "balcão" sem a necessidade de agendamento prévio.
* **Encaixe Inteligente:** O sistema registra esse atendimento sem conflitar com a agenda oficial visual.

### 4. 📊 Business Intelligence e Relatórios
Transforme atendimentos em dados estratégicos.
* **Volumetria:** Quantidade de atendimentos por dia/semana/mês.
* **Taxa de Absenteísmo:** Porcentagem de agendamentos que resultaram em falta.
* **Tipificação:** Quais são os assuntos mais tratados (ex: Reclamação, Elogio, Dúvida Técnica).
* **Comparativo:** Atendimentos Agendados vs. Espontâneos.

---

## 🔄 Fluxo de Trabalho

```mermaid
graph TD
    A[Admin Abre Agenda] -->|Define Horários| B(Calendário Disponível)
    B --> C{Tipo de Acesso}
    C -->|Agendado| D[Reserva de Horário]
    C -->|Espontâneo| E[Registro Imediato]
    D --> F[Dia do Atendimento]
    F --> G{Ocorreu?}
    G -->|Sim| H[Registrar Detalhes e Feedback]
    G -->|Não| I[Marcar como No-Show]
    E --> H
    H --> J[Dashboard de Indicadores]
    I --> J
