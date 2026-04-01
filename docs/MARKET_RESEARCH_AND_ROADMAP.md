# PetTracker: Market Research, Competitive Analysis & Roadmap

_Prepared: March 2026_

---

## 1. Application Overview

PetTracker is an early-stage, open-source .NET 10 REST API designed to track animals across the full shelter lifecycle — intake, fostering, transfer, and adoption. It uses a domain-driven, event-sourced architecture with clean separation between the domain core, access control, and HTTP surface. The current API covers shelter CRUD, sheltered-pet management, inter-shelter pet transfer, and basic history queries.

**Core technical strengths today:**
- Clean DDD-style domain with composable facade interfaces
- Immutable event history per shelter, pet, foster, and adopter entity
- ULID-keyed identity model suitable for distributed storage
- JWT authentication middleware already wired
- Role-based access control adapter present (partially wired to HTTP)
- Full integration test suite with `WebApplicationFactory`

**Current limitations:**
- In-memory data store (no persistence)
- No UI
- Foster, adopter, and rescue group API endpoints are not yet exposed
- Access control not yet enforced at the HTTP layer

---

## 2. Competitive Landscape

### 2.1 Commercial Shelter Management Platforms

| Product | Target Market | Key Capabilities | Pricing Model |
|---|---|---|---|
| **Shelterluv** | Mid-to-large shelters | Intake/outcome workflows, medical records, donor management, public pet search, volunteer scheduling, integrations with Petfinder/AdoptAPet | SaaS subscription (~$200–$600/mo) |
| **PetPoint** (Pethealth) | Large municipal shelters | Full outcome tracking, statistical reporting, Ontario OSPCA compliance, Petfinder sync | Enterprise SaaS |
| **Chameleon** | Municipal & county shelters | Licensing, rabies/vaccine compliance, animal control dispatch, bite/dangerous-animal records | Enterprise SaaS |
| **Pawlytics** | Rescue organizations | Foster management focus, digital adoption contracts, outcome analytics, donor CRM | SaaS (~$50–$150/mo) |
| **Rescue Groups Pro** | Small-to-mid rescues | Foster/adopter management, website builder, application management, Petfinder/AdoptAPet sync | SaaS (~$25–$75/mo) |
| **PetFolio** | Foster networks | Foster onboarding, medical task reminders, photo uploads, communication hub | SaaS |

### 2.2 Open-Source Alternatives

| Project | Technology | Strengths | Weaknesses |
|---|---|---|---|
| **Animal Shelter Manager (ASM3 / sheltermanager.com)** | Python/PostgreSQL (self-hosted or cloud) | Full-featured: medical records, licence management, accounting, reporting, multi-language, Petfinder sync, active community | Monolithic, UI-centric, limited API/developer surface, dated codebase patterns |
| **OpenAdopt** | PHP/MySQL | Basic adoption application workflow | Effectively unmaintained since ~2018 |
| **Petstablished** open tier | SaaS (freemium) | Public pet listings, application forms | Closed source; limited free tier |
| **Adopt-A-Pet API** | REST API (3rd-party data) | Large adoption listing dataset | Read-only, not a management platform |
| **Petfinder Developer API** | REST API (3rd-party data) | 400k+ listed pets, searchable | Read-only listing data; no lifecycle management |

### 2.3 Key Takeaways from Competitive Review

1. **ASM3 is the dominant open-source option** but it is a monolithic web application, not a developer-friendly API. There is no well-maintained open-source REST API for shelter management.
2. **Commercial tools are expensive** (often $300+/month), pricing out small rescues, independent foster networks, and developing-world shelters.
3. **Foster-network management is underserved** — most platforms bolt it on rather than building it as a first-class concept.
4. **Data portability is poor** across the industry; shelters are often locked into proprietary systems with no data export.
5. **Developer ecosystem is thin** — no widely adopted open API standard for pet shelter data exchange exists.

---

## 3. Feature Gap Analysis

The table below maps PetTracker's current state against the capabilities expected in a competitive platform. Gaps are rated by impact (H/M/L) and estimated implementation effort (H/M/L).

| Feature Area | Current State | Gap | Impact | Effort |
|---|---|---|---|---|
| **Persistent storage** | In-memory only | No PostgreSQL/SQLite/CosmosDB backing | H | M |
| **Adoption workflow** | Domain models present, no API | No intake→foster→adopt pipeline exposed | H | M |
| **Medical records** | Not present | Vaccinations, treatments, vet visits, weight log | H | H |
| **Photo / media** | Not present | Pet profile images, video links | H | M |
| **Public pet listings API** | Not present | Searchable public endpoint (breed, age, species, location) | H | M |
| **Petfinder / AdoptAPet sync** | Not present | Outbound syndication to listing aggregators | H | M |
| **Notifications** | Not present | Email/SMS alerts for foster assignments, adoption updates | H | M |
| **Reporting & analytics** | Not present | Intake/outcome rates, length-of-stay, adoption conversion | H | H |
| **Volunteer management** | Not present | Volunteer onboarding, hours tracking, task assignment | M | H |
| **Donor CRM** | Not present | Donation records, donor communication | M | H |
| **Digital contracts** | Not present | Adoption/foster agreements with e-signature | M | M |
| **Mobile API** | No mobile-specific surface | Push notification support, offline-friendly responses | M | M |
| **Role enforcement at HTTP layer** | Access adapter exists, not wired | Role-guarded endpoints not active | H | L |
| **Foster / adopter API endpoints** | Domain exists, endpoints commented out | Foster and adopter lifecycle not callable | H | L |
| **Rescue group API** | Minimal model, no endpoints | Rescue group operations not callable | M | L |
| **Species / breed catalog** | Not present | Structured breed, species, color data | M | L |
| **Intake source tracking** | Not present | Stray, surrender, transfer origin, confiscation | M | M |
| **Outcome tracking** | Not present | Adoption, return to owner, euthanasia, died in care, rescue transfer | H | M |
| **Multi-tenancy** | Single store | Isolation between organizations sharing an instance | M | H |
| **Webhook / event streaming** | Not present | External systems notified on domain events | M | M |
| **Data import/export** | Not present | CSV/JSON import from ASM, PetPoint | M | M |

---

## 4. Suggested New Features (Priority Order)

### Phase 1 — Close Critical Gaps (Weeks 1–6)

1. **Activate foster and adopter endpoints**
   The domain layer already contains `FosterPerson`, `AdopterPerson`, and assignment records. Expose the commented-out routes, wire `ShelterAccessAdapter` to HTTP middleware, and cover with integration tests. This is the highest-value/lowest-effort item in the backlog.

2. **Persistent storage via EF Core**
   Replace the singleton in-memory stores with EF Core repository implementations behind the existing `IDataFacade` interface. Support SQLite for local dev and PostgreSQL for production. The interface boundary makes this a pure infrastructure swap with no domain changes.

3. **Outcome tracking**
   Add `OutcomeKind` enum (`Adopted`, `ReturnedToOwner`, `TransferredToRescue`, `DiedInCare`, `Euthanized`) and surface outcome endpoints. This is the single most-asked feature by shelter staff after basic pet management.

4. **Role enforcement at the HTTP layer**
   `ShelterAccessAdapter` already implements role-checks. Map it into the DI pipeline and replace direct facade calls on mutating endpoints with the adapter. Low effort; high security value.

### Phase 2 — Competitive Parity (Weeks 6–16)

5. **Medical records**
   Introduce a `MedicalEvent` concept (vaccination, de-worming, surgery, vet visit, weight). Attach to `PetIdentity`. Expose CRUD and history endpoints. Consider FHIR-style structure for future interoperability.

6. **Photo / media references**
   Store blob storage URIs (S3/Azure Blob/local path) on pet records. Expose upload-token endpoints to avoid routing binary data through the API itself.

7. **Public search endpoint**
   `GET /pets/search?species=dog&breed=labrador&ageMin=1&location=...` — a read-only, unauthenticated endpoint for public-facing adoption pages and mobile apps.

8. **Petfinder / AdoptAPet outbound sync**
   Implement a background worker that pushes newly available pets to Petfinder via their V2 API. This alone dramatically increases adoption rates for shelters using the platform.

9. **Notification service**
   Abstract an `INotificationService` with email (SMTP/SendGrid) and push implementations. Trigger on key domain events: foster assignment, adoption completion, medical task due.

### Phase 3 — Differentiation (Weeks 16–30)

10. **Intake source & surrender workflow**
    Capture intake source, previous owner details (consent-to-contact flag), surrender reason. Feed into reporting.

11. **Reporting API**
    Aggregate endpoints: length-of-stay histograms, intake/outcome rates by month, breed/species breakdowns, foster network capacity. Return structured data consumable by any BI tool.

12. **Webhook event publishing**
    Publish domain events (`PetListed`, `PetAdopted`, `ShelterCreated`, etc.) to configurable webhook URLs. Enables ecosystem integrations without polling.

13. **Volunteer scheduling**
    Simple shift/task model attached to shelter identity. Scope initially to walks, feeding runs, transport drivers.

14. **Digital adoption agreement**
    Generated PDF agreements with a one-time signing link (DocuSign, Adobe Sign, or self-hosted with PDF generation). Store signed document reference on the adoption record.

15. **Multi-tenancy / organization isolation**
    Introduce `OrganizationIdentity` as a scoping key above `ShelterIdentity`. Required before any meaningful SaaS deployment.

---

## 5. Go-to-Market Plan

### 5.1 Positioning

**Target audiences (in priority order):**

1. **Small independent animal rescues** (5–50 animals in care) that cannot afford $200–$600/month commercial tools and are currently using spreadsheets or Facebook groups.
2. **Developers / tech volunteers** building custom adoption platforms for nonprofits who need a backend API rather than another monolithic app.
3. **Municipal shelters in emerging markets** where English-language commercial SaaS is inaccessible or unaffordable.
4. **University animal welfare programs** deploying a teaching or research tool.

**Positioning statement:**
> PetTracker is the open-source API-first shelter management backend — free to self-host, designed for developers, built for the full pet lifecycle from intake to adoption.

**Differentiators vs. ASM3:**
- Modern API-first architecture (REST / OpenAPI)
- Developer-friendly: clean domain model, easy to extend, integrates with any front-end
- Runs on .NET 10 — cloud-native, container-friendly, runs on Azure/AWS/fly.io in minutes
- No Java/Python runtime dependencies for organizations on Windows/.NET stacks

**Differentiators vs. commercial SaaS:**
- Free and self-hosted — no per-month fees
- Data ownership — complete export at any time
- Extensible — organizations can fork or contribute

### 5.2 Distribution Channels

| Channel | Action | Priority |
|---|---|---|
| **GitHub** | Publish under a permissive license (MIT suggested); invest in README, CONTRIBUTING.md, issue templates, and a project board for the roadmap | Immediate |
| **ProductHunt** | Launch when Phase 1 is complete and a Docker Compose one-liner is available | Phase 1 complete |
| **Hacker News (Show HN)** | Post when public pet search and Petfinder sync are live | Phase 2 complete |
| **r/webdev, r/dotnet, r/animalshelters** | Community posts; emphasize open-source + animal welfare angle | Phase 1 complete |
| **Animal welfare mailing lists** | Direct outreach to ASPCA Technology, Maddie's Fund grants program, HSUS | Phase 2 complete |
| **Dev.to / Medium / .NET Blog** | Technical articles: "Building a domain-driven shelter tracker in .NET 10", "Event sourcing without a framework" | Ongoing |
| **Conference talks** | NDC, .NET Conf, local .NET user groups — present the architecture | Phase 2 complete |
| **Nonprofit tech communities** | NTEN (nonprofit technology network), Tech for Good Slack communities | Phase 2 complete |

### 5.3 Community Building

- **Good first issues** — label beginner-friendly tasks (adding breed catalog, new history query) to attract open-source contributors.
- **Hosted demo instance** — run a free public sandbox at `demo.pettracker.dev` (or similar) with synthetic data, pointing at the Swagger UI. Eliminate the "I have to build it first" barrier for evaluators.
- **Discord / Discussions** — GitHub Discussions as the primary async community channel; a Discord server once contributor count justifies it.
- **Sponsor model** — enable GitHub Sponsors for maintainer sustainability; offer a "supported deployment" paid tier in the future.

### 5.4 Partnerships

| Partner Type | Target | Value |
|---|---|---|
| Petfinder / RescueGroups.org | API integration partnership | Distribution to their existing shelter network |
| Maddie's Fund / ASPCA | Grant funding | Sustainability; credibility in the shelter community |
| Fly.io / Railway / Render | Deploy-button integration | One-click hosting for non-technical shelter staff |
| .NET Foundation | Project membership | Visibility in .NET ecosystem; infrastructure support |

### 5.5 Success Metrics

| Metric | 3-Month Target | 12-Month Target |
|---|---|---|
| GitHub stars | 250 | 2,000 |
| Shelters / rescues actively using | 5 | 50 |
| Contributors (non-author PRs merged) | 3 | 20 |
| Docker Hub pulls | 500 | 5,000 |
| Animals tracked (across deployments) | 500 | 10,000 |

---

## 6. Immediate Next Steps

The following actions can be taken in the next sprint to begin executing the strategy:

1. **Wire foster/adopter HTTP endpoints** — highest ROI code change; completes the core domain surface.
2. **Enable `ShelterAccessAdapter` in the HTTP pipeline** — closes the known security gap.
3. **Add EF Core + SQLite persistence** — makes the application production-deployable for the first time.
4. **Write a `docker-compose.yml`** — enables a one-command local or server deployment.
5. **Draft a public README** — project description, quickstart, API overview, contributing guide, license.
6. **Publish to GitHub (public)** — prerequisite for all community-building activities.
7. **Register on Petfinder developer portal** — obtain API credentials ahead of sync implementation.

---

_Document maintained in `docs/MARKET_RESEARCH_AND_ROADMAP.md`. Update as features ship and competitive landscape evolves._
