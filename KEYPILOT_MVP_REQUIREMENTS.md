# KeyPilot (Home Buyer Copilot)
## MVP Requirements – New Zealand

---

## 1. Product Overview

KeyPilot is a web application that helps buyers manage the **home buying process in New Zealand** from early intent through to settlement.

It is not just a checklist tool.

It is a **guided, workflow-driven system** that helps buyers answer:

- Where am I in the process?
- What should I do next?
- What is my biggest risk right now?

The product must support **real-world buying scenarios**, including:
- multiple properties at once
- different buying methods
- time-sensitive decisions
- legally significant milestones

---

## 2. Core Concept: Workspace

### Definition

A **Workspace** represents a single property purchase journey.

One workspace includes:
- property details
- buying method
- lifecycle stage
- tasks
- reminders
- documents (evidence)
- key dates (offer, conditions, settlement)

A user can have **multiple workspaces active at the same time**.

---

## 3. Supported Buying Methods (MVP)

The system must support the following buying methods:

- Auction
- Negotiation / Asking Price
- Deadline Sale
- Tender
- Private Sale

### Requirement

The system must be **method-aware**:

- behaviour
- guidance
- tasks
- risk messaging
- workflow timing

must adapt based on the selected buying method.

---

## 4. Lifecycle Model

Each workspace progresses through a lifecycle.

### Standard Stages

1. Discovery
2. Offer Preparation
3. Submitted
4. Conditional (if applicable)
5. Unconditional
6. Settlement Pending
7. Settled
8. Archived

### Requirements

- Stage must be **persisted and/or consistently derived**
- Stage must be visible in UI
- Stage transitions must be controlled and predictable
- Some transitions may require explicit confirmation

---

## 5. Tasks

Tasks represent actions the buyer must complete.

### Requirements

Tasks must:
- belong to a workspace
- have a title and description
- have optional due date
- have status (pending / completed / needs attention)
- explain why they matter

Tasks may be:
- mandatory
- recommended
- informational

### Evidence

Some tasks should support:
- document attachment
- notes
- verification status (optional future enhancement)

---

## 6. Reminders & Deadlines

The system must help users avoid missing critical deadlines.

### Requirements

- reminders must be first-class records
- reminders must be tied to:
  - tasks
  - lifecycle events
  - key dates (auction, condition due, settlement)

- reminders must:
  - have due date/time
  - be visible in dashboard
  - increase in urgency as due date approaches

### Behaviour

- reminders must trigger even if user is inactive
- reminder scheduling should support workflow orchestration (Temporal)

---

## 7. Documents (Evidence)

The system must act as a **source of truth for documents**.

### Requirements

Users must be able to:
- upload documents
- associate documents with tasks or workspace
- view/download documents

Documents must:
- belong to a workspace
- be persistently stored
- be retrievable at decision points

---

## 8. Buying Method Behaviour (High Level)

### Auction

- buyer must complete preparation before bidding
- auction represents immediate commitment if won
- system should emphasise readiness before auction day

### Negotiation / Private / Deadline

- buyer submits offer
- offer may include conditions
- if accepted → conditional stage
- buyer completes conditions before going unconditional

### Tender

- buyer submits offer without negotiation
- outcome is delayed
- if selected → proceeds similar to conditional flow

---

## 9. Risk & Decision Points

The system must highlight **high-risk moments**.

### Critical Points

- Auction bidding
- Going unconditional

### Requirements

At these points:
- consequences must be clearly explained
- user must explicitly confirm action
- system must reduce risk of accidental progression

---

## 10. Dashboard Requirements

The workspace dashboard must clearly show:

- current stage
- next critical date
- biggest current risk
- next best action
- key tasks
- relevant reminders

### Principle

The dashboard is **action-oriented**, not just informational.

---

## 11. Multi-Workspace Support

Users must be able to:

- manage multiple workspaces
- see progress across workspaces
- identify urgency across properties
- archive workspaces no longer relevant

---

## 12. Architecture Principles

These must guide implementation:

1. Backend (ASP.NET Core + Postgres) is the **source of truth**
2. Frontend (Next.js) consumes API only
3. Temporal is used for:
   - workflow orchestration
   - timers
   - reminders
4. One workflow per workspace
5. Workflow must not be the only source of truth
6. All user-visible state must be persisted

---

## 13. UX Principles

The product must feel:

- calm
- simple
- structured
- reassuring

### Key UX Rules

- show next action clearly
- highlight risk without overwhelming
- use plain English (NZ context)
- prioritise clarity over density
- mobile-first design
- avoid unnecessary complexity

---

## 14. Out of Scope (MVP)

- bank integrations
- automated credit decisioning
- legal advice automation
- external notifications (email/SMS)
- property valuation/prediction

---

## 15. Success Criteria

The MVP is successful if:

- users complete purchase journeys without losing track
- deadlines are not missed
- users understand key risks before critical decisions
- users actively manage multiple properties
- users feel more confident during the process

---

## 16. Notes for Engineering Agents

When implementing:

- prefer incremental refactoring over rewrite
- preserve working behaviour where possible
- do not assume missing domain concepts
- align new changes to workspace + workflow model
- do not put business truth only in workflow state
- ensure API contracts remain stable or versioned