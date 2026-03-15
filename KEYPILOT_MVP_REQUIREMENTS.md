# KEYPILOT_MVP_REQUIREMENTS.md

# KeyPilot MVP Requirements
Version: 0.1
Scope: Accepted Offer → Settlement
Product: KeyPilot
Primary channel: Responsive web app (mobile-first)
Architecture target: Next.js frontend + .NET modular monolith backend + Postgres

---

## 1. Product Goal

KeyPilot helps home buyers in New Zealand manage the period from accepted offer to settlement.

The MVP is not a property search tool, mortgage marketplace, or legal platform.
It is a transaction management app that helps buyers understand:
- what stage they are in
- what conditions are active
- what tasks they need to complete next
- what deadlines are approaching
- how close they are to settlement

---

## 2. MVP Scope

### In scope
- Create a purchase workspace after an offer has been accepted
- Capture key purchase details
- Capture and track purchase conditions
- Auto-generate tasks from selected conditions
- Display the purchase stage
- Display deadlines and next tasks
- Allow tasks to be marked complete
- Allow conditions to be marked complete
- Progress the purchase from accepted offer to settlement
- Basic dashboard for one purchase
- Responsive web UI designed mobile-first

### Out of scope
- Property search
- Offer creation
- Mortgage pre-approval
- Bank integrations
- Lawyer integrations
- Real estate portal integrations
- Insurance integrations
- Inspection booking
- AI document summaries
- Push notifications
- SMS
- Multi-property comparison
- Partner marketplace
- Workflow builder / generic rules engine

---

## 3. Core User Problem

After an offer is accepted, buyers often manage the transaction through:
- email
- PDFs
- notes
- spreadsheets
- calls with lawyer / broker / agent

The MVP solves:
- “What do I need to do next?”
- “What conditions are due soon?”
- “Where am I in the process?”
- “Am I ready for settlement?”

---

## 4. Primary User

### Primary persona
Home buyer in New Zealand with an accepted offer who needs help managing the transaction through to settlement.

### Assumptions
- User has already found the property
- User has an accepted offer or signed agreement
- User knows or can enter the settlement date
- User knows which conditions apply, or can select from a standard list

---

## 5. Product Boundary

### Start
Offer accepted

### End
Settlement complete

The application lifecycle begins only after the user has an accepted offer.

---

## 6. Purchase Lifecycle Stages

The MVP must support these stages:
1. accepted_offer
2. conditional
3. unconditional
4. pre_settlement
5. settled

### Stage meaning

#### accepted_offer
Offer has been accepted and purchase has been created in the app.

#### conditional
One or more conditions are still active and not yet completed.

#### unconditional
All conditions are complete or waived, and the purchase is moving toward settlement.

#### pre_settlement
Final tasks before settlement are being completed.

#### settled
Funds transferred, keys collected, transaction complete.

---

## 7. Functional Requirements

## 7.1 Create Purchase Workspace

The user must be able to create a purchase workspace with the following fields:

### Required
- property address
- accepted offer date
- settlement date

### Optional
- purchase price
- deposit amount

### Conditions
User can select one or more standard conditions:
- finance
- building_report
- lim
- insurance
- solicitor_approval

### Condition due dates
For MVP, the system can either:
- accept explicit due dates, or
- calculate due dates from defaults

Recommended default offsets from accepted offer date:
- finance: 5 days
- building_report: 5 days
- lim: 10 days
- insurance: 10 days
- solicitor_approval: 5 days

### System behaviour on create
When a purchase is created, the system must:
1. create the purchase record
2. create selected condition records
3. calculate stage
4. generate tasks from selected conditions
5. generate standard pre-settlement tasks
6. return the full purchase summary

---

## 7.2 View Purchase Dashboard

The user must be able to view a dashboard for a purchase showing:
- property address
- purchase status / stage
- accepted offer date
- settlement date
- days until settlement
- active conditions
- overdue / upcoming condition deadlines
- next tasks
- task completion summary

Dashboard goal:
- give the user a clear understanding of current stage and next required actions

---

## 7.3 Conditions Tracking

The user must be able to view a list of conditions for a purchase.

For each condition, the system must show:
- condition type
- due date
- status
- completed date if completed

Condition statuses:
- pending
- completed
- expired

The user must be able to:
- mark a condition complete

### Stage logic related to conditions
- If one or more conditions are pending, purchase stage must be conditional
- If all conditions are completed and settlement has not happened, purchase stage must move to unconditional or pre_settlement based on current logic

---

## 7.4 Tasks

The system must auto-generate tasks.

Each task must include:
- title
- stage
- due date optional
- status
- linked condition optional

Task statuses:
- pending
- completed

Task stages:
- accepted_offer
- conditional
- unconditional
- pre_settlement
- settlement

The user must be able to:
- view tasks grouped by stage
- mark tasks complete
- view next upcoming tasks

The user does not need to:
- re-order tasks
- assign tasks to other users
- comment on tasks
- upload files against tasks in MVP

---

## 7.5 Stage Progression

The system must support stage progression.

### Recommended rules
#### accepted_offer → conditional
When purchase is created with one or more active conditions

#### accepted_offer → pre_settlement
When purchase is created with no conditions

#### conditional → unconditional
When all conditions are completed

#### unconditional → pre_settlement
When all conditions are completed and there are remaining pre-settlement tasks

#### pre_settlement → settled
When settlement is marked complete

### Manual actions
User must be able to:
- mark settlement complete

The system can determine other stage changes automatically.

---

## 7.6 Purchase Detail View

The purchase detail page must show:
- address
- status
- accepted offer date
- settlement date
- purchase price
- selected conditions
- generated tasks
- progress summary

---

## 8. Task Generation Requirements

For MVP, do not build a generic workflow engine.

Use a simple template-driven approach:
- selected conditions
- map to task templates
- generate task instances

### 8.1 Accepted Offer tasks
Always generate:
- Confirm lawyer details
- Review active conditions
- Confirm settlement date

### 8.2 Finance condition tasks
Generate:
- Submit finance documents
- Confirm lender approval

### 8.3 Building report condition tasks
Generate:
- Book building inspection
- Review building report

### 8.4 LIM condition tasks
Generate:
- Obtain LIM report
- Review LIM findings

### 8.5 Insurance condition tasks
Generate:
- Obtain insurance quote
- Confirm insurance cover

### 8.6 Solicitor approval condition tasks
Generate:
- Send agreement to lawyer
- Confirm solicitor approval

### 8.7 Pre-settlement tasks
Always generate:
- Confirm final loan approval
- Confirm insurance active from settlement date
- Complete final inspection
- Confirm settlement funds ready
- Confirm key collection details

### 8.8 Settlement tasks
Always generate:
- Lawyer confirms settlement
- Funds transferred
- Keys collected

---

## 9. Non-Functional Requirements

### 9.1 Architecture
The MVP must be implemented as:
- Next.js frontend
- .NET modular monolith backend
- Postgres database

Do not use:
- microservices
- Kafka
- event sourcing
- distributed workflow engines

### 9.2 Performance
The app should feel responsive for normal consumer use.
No special scale patterns are required for MVP.

### 9.3 Maintainability
Backend modules must be separated logically to allow future growth into:
- documents
- contacts
- notifications
- marketplace integrations

### 9.4 Security
Basic good practice only for MVP:
- validate inputs
- use server-side API boundaries
- do not expose internal DB details
- no auth implementation required yet unless already scaffolded

---

## 10. Backend Domain Requirements

## 10.1 Current implementation note
If Property already exists, the team may continue using Property as the aggregate root for speed.

Longer term this may split into Property and Purchase, but that refactor is not required for MVP.

### Recommended pragmatic model for MVP
Use Property as the main aggregate, extended to represent the accepted-offer-to-settlement transaction.

Fields:
- id
- address
- accepted_offer_date
- settlement_date
- purchase_price nullable
- deposit_amount nullable
- status
- created_at_utc

Associated child entities:
- conditions
- tasks

---

## 10.2 Entities

### Property
Fields:
- id
- address
- accepted_offer_date
- settlement_date
- purchase_price
- deposit_amount
- status
- created_at_utc

### Condition
Fields:
- id
- property_id
- type
- due_date
- status
- completed_at_utc nullable
- created_at_utc

### Task
Fields:
- id
- property_id
- condition_id nullable
- title
- stage
- due_date nullable
- status
- completed_at_utc nullable
- created_at_utc

---

## 10.3 Enums

### PropertyStatus
- accepted_offer
- conditional
- unconditional
- pre_settlement
- settled

### ConditionType
- finance
- building_report
- lim
- insurance
- solicitor_approval

### ConditionStatus
- pending
- completed
- expired

### TaskStatus
- pending
- completed

### TaskStage
- accepted_offer
- conditional
- unconditional
- pre_settlement
- settlement

---

## 11. API Requirements

Base route:
- /api/v1

### 11.1 Create Property / Purchase
POST /api/v1/properties

Request body:
```json
{
  "address": "12 Beach Road, Takapuna",
  "acceptedOfferDate": "2026-03-16",
  "settlementDate": "2026-04-30",
  "purchasePrice": 1200000,
  "depositAmount": 200000,
  "conditions": [
    { "type": "finance", "daysFromAcceptedOffer": 5 },
    { "type": "building_report", "daysFromAcceptedOffer": 5 },
    { "type": "lim", "daysFromAcceptedOffer": 10 }
  ]
}
```

Response body:
```json
{
  "id": "guid",
  "address": "12 Beach Road, Takapuna",
  "status": "conditional",
  "acceptedOfferDate": "2026-03-16",
  "settlementDate": "2026-04-30",
  "purchasePrice": 1200000,
  "depositAmount": 200000,
  "conditions": [
    {
      "id": "guid",
      "type": "finance",
      "dueDate": "2026-03-21",
      "status": "pending"
    }
  ],
  "tasks": [
    {
      "id": "guid",
      "title": "Submit finance documents",
      "stage": "conditional",
      "status": "pending"
    }
  ]
}
```

### 11.2 Get Property / Purchase
GET /api/v1/properties/{id}

Must return:
- purchase summary
- conditions
- tasks

### 11.3 Complete Task
PATCH /api/v1/tasks/{id}/complete

Must:
- mark task complete
- return updated task or success result

### 11.4 Complete Condition
PATCH /api/v1/conditions/{id}/complete

Must:
- mark condition complete
- recalculate purchase stage if needed
- return updated condition or success result

### 11.5 Mark Settlement Complete
PATCH /api/v1/properties/{id}/settle

Must:
- mark purchase settled
- update status to settled

---

## 12. Frontend Requirements

### 12.1 Pages

#### Home page /
Purpose:
- lightweight landing page
- CTA to create purchase

#### Create purchase page /properties/new
Must include:
- property address
- accepted offer date
- settlement date
- purchase price optional
- deposit amount optional
- condition selection

#### Property detail page /properties/[id]
Must include:
- property summary
- purchase stage
- days until settlement
- conditions section
- tasks section
- progress indicators

---

## 12.2 UX Requirements

The UI must be mobile-first.

The property detail page should answer immediately:
- What stage am I in?
- What is due next?
- How many days until settlement?
- Which conditions are still pending?
- What tasks remain?

Recommended sections:
1. header summary
2. current stage
3. condition cards
4. next tasks
5. all tasks grouped by stage

---

## 12.3 Validation

The frontend must validate:
- address required
- accepted offer date required
- settlement date required
- settlement date must not be before accepted offer date

---

## 13. Agent Build Sequence

Agents should build in this order.

### Agent 1 — Domain and API
Build:
- Property extended for accepted-offer-to-settlement lifecycle
- Condition entity
- Task entity
- enums
- EF Core mappings
- migrations
- create/get/complete endpoints

### Agent 2 — Task generation logic
Build:
- task template service
- condition-driven task creation
- stage determination logic

### Agent 3 — Frontend create flow
Build:
- create purchase form
- condition selection UI
- POST integration

### Agent 4 — Frontend detail dashboard
Build:
- property detail page
- condition rendering
- task rendering
- basic complete task / complete condition actions

---

## 14. Acceptance Criteria

The MVP requirement is satisfied when:
1. User can create a purchase from accepted offer stage
2. User can select one or more conditions
3. Backend creates conditions and tasks automatically
4. Property detail page displays purchase summary
5. Property detail page displays purchase stage
6. Property detail page displays condition deadlines
7. Property detail page displays generated tasks
8. User can mark a task complete
9. User can mark a condition complete
10. Purchase stage updates correctly
11. User can mark settlement complete
12. Purchase moves to settled

---

## 15. Explicit Simplifications for MVP

To keep agents focused, the following simplifications are intentional:
- single-user use case
- one property / purchase flow at a time
- no collaboration
- no auth dependency
- no document storage yet
- no contact management yet
- no notifications yet
- no calendar logic beyond simple date calculation
- no working day calendar unless team wants to add later; plain day offsets are acceptable for MVP

---

## 16. Notes for Agents

- Keep code simple and readable
- Do not introduce a generic workflow engine
- Use hard-coded task templates behind a small service interface
- Do not refactor into microservices
- Prefer a clean working vertical slice over abstract future-proof frameworks
- The core product value is conditions + tasks + stage tracking from accepted offer to settlement
