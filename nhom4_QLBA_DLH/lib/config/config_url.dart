import 'package:flutter_dotenv/flutter_dotenv.dart';

class Config_URL {
  static String get baseUrl {
    final url = dotenv.env['BASE_URL'];
    if (url == null) {
      print("BASE_URL is not set in the .env file. Using default URL.");
          //đường dẫn API nếu không đọc được URL trong file              .env
          return
          "https://10.0.2.2:7290/";
      }
          return url;
      }
}