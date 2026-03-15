# System Design

## Core Entities

Workspace
- id
- name
- owner

Property
- id
- workspace_id
- address
- status
- offer_accepted_date
- settlement_date

Condition
- id
- property_id
- type
- due_date
- status

Task
- id
- property_id
- condition_id
- title
- due_date
- status

Document
- id
- property_id
- storage_key
- file_name
- category

Contact
- id
- property_id
- role
- name
- email
- phone
