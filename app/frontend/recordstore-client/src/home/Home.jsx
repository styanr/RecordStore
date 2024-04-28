import { useEffect, useState } from 'react';
import {
  Center,
  Heading,
  Text,
  Card,
  CardHeader,
  CardBody,
  CardFooter,
  Grid,
  GridItem,
  Image,
  LinkBox,
  LinkOverlay,
  CircularProgress,
  Flex,
  Box,
  Input,
  RangeSlider,
  RangeSliderTrack,
  RangeSliderFilledTrack,
  RangeSliderThumb,
  Button,
  Container,
} from '@chakra-ui/react';

import { SearchIcon } from '@chakra-ui/icons';

import {
  AutoComplete,
  AutoCompleteInput,
  AutoCompleteItem,
  AutoCompleteList,
} from '@choc-ui/chakra-autocomplete';

import { Link as ReactRouterLink } from 'react-router-dom';
import productService from './ProductService';
import genreService from './GenreService';
import formatService from './FormatService';

const Home = () => {
  const [products, setProducts] = useState([]);
  const [search, setSearch] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const [title, setTitle] = useState('');
  const [artist, setArtist] = useState('');
  const [genre, setGenre] = useState('');
  const [format, setFormat] = useState('');
  const [minPrice, setMinPrice] = useState(0);
  const [maxPrice, setMaxPrice] = useState(0);
  const [page, setPage] = useState(1);

  const [totalPages, setTotalPages] = useState(0);
  const [totalCount, setTotalCount] = useState(0);

  const [prices, setPrices] = useState({});

  useEffect(() => {
    const fetchPrices = async () => {
      const prices = await productService.getMinMaxPrices();
      setPrices(prices);
    };

    fetchPrices();
  }, []);

  useEffect(() => {
    const fetchProducts = async () => {
      const filterParams = {};

      if (title) {
        filterParams.Title = title;
      }
      if (title !== '') {
        filterParams.Title = title;
      }
      if (artist !== '') {
        filterParams.Artist = artist;
      }
      if (genre !== '') {
        filterParams.Genre = genre;
      }
      if (format !== '') {
        filterParams.Format = format;
      }
      if (minPrice !== 0) {
        filterParams.MinPrice = minPrice;
      }
      if (maxPrice !== 0) {
        filterParams.MaxPrice = maxPrice;
      }
      filterParams.Page = page;

      const products = await productService.getProducts(filterParams);
      setProducts(products.results);
      setTotalPages(products.pageCount);
      setTotalCount(products.rowCount);
    };

    setIsLoading(true);
    fetchProducts();
    setIsLoading(false);
  }, [search]);

  const [genreOptions, setGenreOptions] = useState([]);
  const [genreSearch, setGenreSearch] = useState('');
  const [isGenreLoading, setIsGenreLoading] = useState(false);

  const [formatOptions, setFormatOptions] = useState([]);
  const [formatSearch, setFormatSearch] = useState('');
  const [isFormatLoading, setIsFormatLoading] = useState(false);

  useEffect(() => {
    const fetchFormats = async () => {
      const formats = await formatService.searchFormats(formatSearch);
      console.log(formats);
      setFormatOptions(formats);
    };

    setIsFormatLoading(true);
    fetchFormats();
    setIsFormatLoading(false);
  }, [formatSearch]);

  useEffect(() => {
    const fetchGenres = async () => {
      const genres = await genreService.getGenres(genreSearch);
      console.log(genres);
      setGenreOptions(genres);
    };

    setIsGenreLoading(true);
    fetchGenres();
    setIsGenreLoading(false);
  }, [genreSearch]);

  const handleSearch = () => {
    setSearch(!search);
  };

  return (
    <Box bg='gray.100' minH='100vh' py={12}>
      {isLoading ? (
        <Center>
          <CircularProgress isIndeterminate size='60px' color='blue.500' />
        </Center>
      ) : (
        <Container maxW='7xl'>
          <Flex justify='space-between' align='center' mb={8}>
            <Heading size='xl' fontWeight='bold'>
              Знайдено {totalCount} записів
            </Heading>
            <Button
              colorScheme='blue'
              onClick={handleSearch}
              rightIcon={<SearchIcon />}
            >
              Пошук
            </Button>
          </Flex>
          <Flex>
            <Box bg='white' borderRadius='md' boxShadow='md' p={6} mr={8}>
              <Heading size='md' mb={4}>
                Фільтри
              </Heading>
              <Input
                placeholder='Назва'
                value={title}
                onChange={(e) => setTitle(e.target.value)}
                mb={4}
              />
              <Input
                placeholder='Виконавець'
                value={artist}
                onChange={(e) => setArtist(e.target.value)}
                mb={4}
              />
              <Flex gap={4} mb={4}>
                <AutoComplete openOnFocus isLoading={isGenreLoading} mb={4}>
                  <AutoCompleteInput
                    placeholder='Формат'
                    onChange={(e) => {
                      if (e.target.value === '') {
                        setFormat('');
                      }
                      setFormatSearch(e.target.value);
                    }}
                  />
                  <AutoCompleteList>
                    {formatOptions.map((option, index) => (
                      <AutoCompleteItem
                        key={index}
                        onClick={() => setFormat(option.name)}
                        value={option.name}
                      >
                        {option.name}
                      </AutoCompleteItem>
                    ))}
                  </AutoCompleteList>
                </AutoComplete>
                <AutoComplete openOnFocus isLoading={isGenreLoading} mb={4}>
                  <AutoCompleteInput
                    placeholder='Жанр'
                    onChange={(e) => {
                      if (e.target.value === '') {
                        setGenre('');
                      }
                      setGenreSearch(e.target.value);
                    }}
                  />
                  <AutoCompleteList>
                    {genreOptions.map((option) => (
                      <AutoCompleteItem
                        key={option.name}
                        onClick={() => setGenre(option.name)}
                        value={option.name}
                      >
                        {option.name}
                      </AutoCompleteItem>
                    ))}
                  </AutoCompleteList>
                </AutoComplete>
              </Flex>
              <Flex justify='space-between' align='center' mb={4}>
                <Text>Ціна: {minPrice} ₴</Text>
                <Text>{maxPrice} ₴</Text>
              </Flex>
              <RangeSlider
                min={prices.minPrice}
                max={prices.maxPrice}
                step={1}
                defaultValue={[minPrice, maxPrice]}
                onChange={(values) => {
                  setMinPrice(values[0]);
                  setMaxPrice(values[1]);
                }}
                mb={4}
              >
                <RangeSliderTrack>
                  <RangeSliderFilledTrack bg='blue.500' />
                </RangeSliderTrack>
                <RangeSliderThumb boxSize={6} index={0} />
                <RangeSliderThumb boxSize={6} index={1} />
              </RangeSlider>
            </Box>
            <Grid
              templateColumns={[
                'repeat(1, 1fr)',
                'repeat(2, 1fr)',
                'repeat(3, 1fr)',
              ]}
              gap={6}
              flex='1'
            >
              {products.length === 0 && (
                <Center>
                  <Text>Нічого не знайдено</Text>
                </Center>
              )}
              {products.map((product, index) => (
                <Box
                  bg='white'
                  borderRadius='md'
                  boxShadow='md'
                  overflow='hidden'
                  key={index}
                  _hover={{ boxShadow: 'xl', transform: 'translateY(-5px)' }}
                  transition={'all 0.2s ease-in-out'}
                >
                  <LinkBox as='article'>
                    <Image
                      src={product.image_url}
                      fallbackSrc='https://via.placeholder.com/150'
                      alt='product image'
                      objectFit='cover'
                      maxH='240px'
                      w='100%'
                    />
                    <LinkOverlay
                      as={ReactRouterLink}
                      to={`/products/${product.id}`}
                    >
                      <Box p={4}>
                        <Heading size='sm' fontWeight='semibold'>
                          {product.title}
                        </Heading>
                        <Text fontSize='sm' mt={1} color='gray.500'>
                          {product.format.name}
                        </Text>
                        <Text fontSize='sm' mt={1}>
                          {product.artists.map((artist, index) =>
                            index === 0 ? (
                              <span key={index}>{artist.name}</span>
                            ) : (
                              <span key={index}>, {artist.name}</span>
                            )
                          )}
                        </Text>
                      </Box>
                    </LinkOverlay>
                    <Flex
                      align='center'
                      justify='space-between'
                      bg='gray.50'
                      p={4}
                      borderTopWidth={1}
                      borderTopColor='gray.200'
                    >
                      <Text color='blue.500' fontWeight='bold'>
                        {product.price} ₴
                      </Text>
                    </Flex>
                  </LinkBox>
                </Box>
              ))}
            </Grid>
          </Flex>
        </Container>
      )}
    </Box>
  );
};

export default Home;
