---
trigger: always_on
---

###

- Khi được yêu cầu load component nào đó thì luôn luôn tạo 1 function load component rồi call nó trong function override LoadComponents.
- Luôn tạo document cho Class.
- Khi tạo singleton thì tạo một function cho singleton rồi gọi nó trong LoadComponents.
- Luôn dùng this. cho các biến internal.
- Nếu được thì ưu tiên dùng if kiểu Guard Clause thay vì kiểu truyền thống.
- Không dùng while(true) {}.
- Không viết logic vào các method của Unity như Start(), Update(), FixedUpdate() v.v... cũng như các method trong MasterBehaviour, thay vào đó viết function riêng biệt.
- Không viết tắt kiểu như SpriteRenderer sr mà viết là SpriteRenderer spriteRenderer.
- Khi viết code setup chỉ chạy ở editor thì dùng partial viết ở 1 file riêng, hậu tố của file sẽ là Setup, ví dụ như class PlanetSpawner thì file setup sẽ là PlanetSpawnerSetup
- Luôn viết comment bằng tiếng anh

### Commit code

- Chỉ commit code khi được yêu cầu, và tôi sẽ yêu cầu commit và chỉ ra type cần cần commit
- Format cho commit message: [type][id]commit_message
  - type: feat || fix || update
  - commit_message là tóm tắt ngắn gọn những gì làm trong commit đó, và luôn dùng tiếng anh
  - id: tạo 1 mã hash 7 kí từ gồm sô và chứ cái viết thường
